using System;
using Code.Interface.Settings;
using Code.Managers;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class NewMovement : NetworkBehaviour{
        [Header("States")] public bool grounded;

        [Header("Slope & Ground Handling")] public float maxSlopeAngle;
        public LayerMask groundLayer;

        [Header("Movement Settings")] public float maxSpeed;
        public float moveSpeed = 4500;
        public float counterMovement =  0.175f;
        public float jumpCooldown;

        [Header("Mouse Settings")] public bool invertX;
        public float xSensitivity = 1;
        [Space] public bool invertY = true;
        public float ySensitivity = 1;

        [Header("Keyboard Settings")] public KeyCode forward = KeyCode.W;
        public KeyCode back = KeyCode.S;
        public KeyCode left = KeyCode.A;
        public KeyCode right = KeyCode.D;
        [Space] public KeyCode crouch = KeyCode.LeftControl;
        [Space] public KeyCode jump = KeyCode.Space;

        [Header("Rotation")] public Transform orientation;
        public Transform cameraHolder;
        public Transform colliderTransform;

        private float _xMouse, _yMouse;
        private float _xKeyboardRaw, _yKeyboardRaw;
        private bool _cancelingGrounded, _canJump;
        private Vector3 _groundNormal;
        private Rigidbody _rb;
        private GamePlayer _gamePlayer;
        private CameraController _cameraController;
        private float _lastGrounded;
        private float _goalCrouchHeight, _currentCrouchHeight;
        private Vector3 _colliderSize;
        private const float Threshold = 0.01f;

        private void Start(){
            _rb = GetComponent<Rigidbody>();
            _gamePlayer = GetComponent<GamePlayer>();
            _cameraController = GetComponent<CameraController>();

            _canJump = true;

            _colliderSize = colliderTransform.localScale;
            _currentCrouchHeight = 1;
            _goalCrouchHeight = 1;

            if (!isLocalPlayer) return;
            SettingsMenu.Singleton.LoadingSettings.AddListener(LoadSettings);
            LoadSettings();
        }

        private void FixedUpdate(){
            if (!isLocalPlayer) return;
            Movement();
        }

        private void Update(){
            if (!isLocalPlayer) return;

            UpdateInputs();
            UpdateRotations();
        }

        private void LoadSettings(){
            //sens x
            if (PlayerPrefs.HasKey("sens_x")){
                xSensitivity = PlayerPrefs.GetFloat("sens_x");
            }
            else{
                xSensitivity = 1;
                PlayerPrefs.SetFloat("sens_x", xSensitivity);
            }

            //sens y
            if (PlayerPrefs.HasKey("sens_y")){
                ySensitivity = PlayerPrefs.GetFloat("sens_y");
            }
            else{
                ySensitivity = 1;
                PlayerPrefs.SetFloat("sens_y", ySensitivity);
            }

            if (PlayerPrefs.HasKey("forward")){
                forward = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("forward"));
            }
            else{
                PlayerPrefs.SetInt("forward", (int)forward);
            }

            if (PlayerPrefs.HasKey("left")){
                left = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("left"));
            }
            else{
                PlayerPrefs.SetInt("left", (int)left);
            }

            if (PlayerPrefs.HasKey("right")){
                right = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("right"));
            }
            else{
                PlayerPrefs.SetInt("right", (int)right);
            }

            if (PlayerPrefs.HasKey("back")){
                back = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("back"));
            }
            else{
                PlayerPrefs.SetInt("back", (int)back);
            }

            if (PlayerPrefs.HasKey("jump")){
                jump = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("jump"));
            }
            else{
                PlayerPrefs.SetInt("jump", (int)jump);
            }

            if (PlayerPrefs.HasKey("crouch")){
                crouch = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("crouch"));
            }
            else{
                PlayerPrefs.SetInt("crouch", (int)crouch);
            }
        }

        private void UpdateRotations(){
            orientation.rotation = Quaternion.Euler(0, _xMouse, 0);
            cameraHolder.rotation = Quaternion.Euler(_yMouse, _xMouse, 0);
        }

        private void UpdateInputs(){
            //If a window is open zero all inputs
            if (CursorManager.Singleton.WindowsOpend){
                _xKeyboardRaw = 0;
                _yKeyboardRaw = 0;
                return;
            }

            //Update all inputs
            _xMouse += Input.GetAxis("Mouse X") * xSensitivity * (invertX ? -1 : 1);
            _yMouse += Input.GetAxis("Mouse Y") * ySensitivity * (invertY ? -1 : 1);

            _yMouse = Mathf.Clamp(_yMouse, -85, 85);

            //If player is frozen zero the keyboard inputs
            if (_gamePlayer.frozen){
                _xKeyboardRaw = 0;
                _yKeyboardRaw = 0;
                return;
            }

            _xKeyboardRaw = 0;
            if (Input.GetKey(right))
                _xKeyboardRaw += 1;
            if (Input.GetKey(left))
                _xKeyboardRaw -= 1;

            _yKeyboardRaw = 0;
            if (Input.GetKey(forward))
                _yKeyboardRaw += 1;
            if (Input.GetKey(back))
                _yKeyboardRaw -= 1;
        }

        private void Movement(){
            //Extra gravity
            _rb.AddForce(Vector3.down * (Time.deltaTime * 10));

            //Find actual velocity relative to where player is looking
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            //Counteract sliding and sloppy movement
            CounterMovement(_xKeyboardRaw, _yKeyboardRaw, mag);
            

            //If sliding down a ramp, add force down so player stays grounded and also builds speed
            //if (crouching && grounded && _canJump){
            //    rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            //    return;
            //}

            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (_xKeyboardRaw > 0 && xMag > maxSpeed) _xKeyboardRaw = 0;
            if (_xKeyboardRaw < 0 && xMag < -maxSpeed) _xKeyboardRaw = 0;
            if (_yKeyboardRaw > 0 && yMag > maxSpeed) _yKeyboardRaw = 0;
            if (_yKeyboardRaw < 0 && yMag < -maxSpeed) _yKeyboardRaw = 0;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Movement in air
            if (!grounded){
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }

            // Movement while sliding
            //if (grounded && crouching) multiplierV = 0f;

            //Apply forces to move player
            _rb.AddForce(orientation.forward * (_yKeyboardRaw * moveSpeed * Time.deltaTime * multiplier * multiplierV));
            _rb.AddForce(orientation.right * (_xKeyboardRaw * moveSpeed * Time.deltaTime * multiplier));
        }

        private Vector2 FindVelRelativeToLook() {
            float lookAngle = orientation.eulerAngles.y;
            float moveAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle);
            float v = 90 - u;

            float magnitude = _rb.velocity.magnitude;
            float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
            float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        
            return new Vector2(xMag, yMag);
        }
        
        private void CounterMovement(float x, float y, Vector2 mag) {
            if (!grounded) return;

            //Slow down sliding
            //if (crouching) {
            //    rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            //    return;
            //}

            //Counter movement
            if (Math.Abs(mag.x) > Threshold && Math.Abs(x) < 0.05f || (mag.x < -Threshold && x > 0) || (mag.x > Threshold && x < 0)) {
                _rb.AddForce(orientation.right * (moveSpeed * Time.deltaTime * -mag.x * counterMovement));
            }
            if (Math.Abs(mag.y) > Threshold && Math.Abs(y) < 0.05f || (mag.y < -Threshold && y > 0) || (mag.y > Threshold && y < 0)) {
                _rb.AddForce(orientation.forward * (moveSpeed * Time.deltaTime * -mag.y * counterMovement));
            }
        
            //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
            if (Mathf.Sqrt((Mathf.Pow(_rb.velocity.x, 2) + Mathf.Pow(_rb.velocity.z, 2))) > maxSpeed) {
                float fallspeed = _rb.velocity.y;
                Vector3 n = _rb.velocity.normalized * maxSpeed;
                _rb.velocity = new Vector3(n.x, fallspeed, n.z);
            }
        }


        private bool IsFloor(Vector3 v){
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }

        private void OnCollisionStay(Collision other){
            if (!isLocalPlayer) return;

            //Make sure we are only checking for walkable layers
            int layer = other.gameObject.layer;
            if (groundLayer != (groundLayer | (1 << layer))) return;

            //Iterate through every collision in a physics update
            for (int i = 0; i < other.contactCount; i++){
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (!IsFloor(normal)) continue;
                //isOnSlope = IsSlope(normal);
                if (!grounded && _lastGrounded < Time.time - jumpCooldown){
                    _cameraController.SetPitch(15);
                }

                grounded = true;
                _cancelingGrounded = false;
                _groundNormal = normal;
                CancelInvoke(nameof(StopGrounded));
            }

            //Invoke ground/wall cancel, since we can't check normals with CollisionExit
            const float delay = 3f;
            if (_cancelingGrounded) return;
            _cancelingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }

        private void StopGrounded(){
            grounded = false;
            _lastGrounded = Time.time;
            //isOnSlope = false;
            _groundNormal = Vector3.up;
        }
    }
}