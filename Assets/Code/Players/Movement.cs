using System;
using Code.Interface.Settings;
using Code.Managers;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class Movement : NetworkBehaviour{
        public bool grounded;
        public bool sliding;
        public bool crouching;
        public bool isOnSlope;
        [Header("General Settings")] 
        public LayerMask groundLayers = 0;
        public float minSlopeAngle = 15f;
        public float maxSlopeAngle = 45f;
        public bool canSlide;
        [Header("Movement Settings")] 
        public float walkSpeed = 70f;
        public float slideSpeed = 125;
        public float maxSlideSpeed;
        public float crouchSpeed = 30f;
        public float counterMovementForce = 10f;
        [Space] public float jumpForce;
        public float jumpCooldown;
        [Range(0, 1)] public float mimicGroundAngle;
        [Space, Range(0, 1)] public float airControll;
        private bool _startedWalking;
        public float BaseMoveSpeed => CurrentMoveSpeed / counterMovementForce;

        private float CurrentMoveSpeed {
            get{
                if (!grounded) return walkSpeed;
                if (sliding ) return (isOnSlope && _rb.velocity.y < 0) ? SlopeSpeed() : walkSpeed;
                return crouching ? crouchSpeed : walkSpeed;
            }
        }

        [Header("Crouch Settings")] 
        public float slideTime;
        public float crouchTransitionSpeed = 10;
        public float targetCrouchHeight;


        [Header("Keyboard Settings")] public KeyCode forward = KeyCode.W;
        public KeyCode back = KeyCode.S;
        public KeyCode left = KeyCode.A;
        public KeyCode right = KeyCode.D;
        [Space] public KeyCode crouch = KeyCode.LeftControl;
        [Space] public KeyCode jump = KeyCode.Space;
        [Space] public bool useInterpolation;
        public float interpolationSpeed;

        [Header("Mouse Settings")] public bool invertX;
        public float xSensitivity = 1;
        [Space] public bool invertY = true;
        public float ySensitivity = 1;

        [Header("References")] public Transform orientation;
        public Transform colliderTransform;
        public Transform cameraHolder;
        public Transform rotation;
        public Animator animator;
        public Animator handsAnimator;

        private Rigidbody _rb;
        private GamePlayer _gamePlayer;
        private CameraController _cameraController;
        
        private float _xMouse, _yMouse;
        private float _xKeyboard, _xKeyboardRaw, _yKeyboard, _yKeyboardRaw;
        private bool _cancelingGrounded, _canJump;
        
        private Vector3 _groundNormal;
        private float _angle;
        
        private float _lastGrounded;
        
        private float _goalCrouchHeight, _currentCrouchHeight;
        private Vector3 _colliderSize;
        
        private float _slideTimer;
        private float _timeSinceSlopeStarted;
        private bool _slideTimerStarted;
        
        private void OnGUI(){
            Rect position = new Rect{
                center = new Vector2(Screen.width / 2f, 0),
                height = 50,
                width = 500,
            };
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;
            GUI.Label(position, $"{Mathf.Round(velocity.magnitude * 10)/10}");
        }

        private void Start(){
            _rb = GetComponent<Rigidbody>();
            _gamePlayer = GetComponent<GamePlayer>();
            _cameraController = GetComponent<CameraController>();
            _canJump = true;
            _colliderSize = colliderTransform.localScale;

            SettingsMenu.Singleton.LoadingSettings.AddListener(LoadSettings);
            LoadSettings();

            _currentCrouchHeight = 1;
            _goalCrouchHeight = 1;
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

        

        private void FixedUpdate(){
            if (!isLocalPlayer) return;

            Move();
            CalculateAnimation();
            
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;
            if (sliding && velocity.magnitude < BaseMoveSpeed / 2){
                sliding = false;
            }

            if (sliding){
                if ((!isOnSlope || _rb.velocity.y > 0) && !_slideTimerStarted){
                    _slideTimer = Time.time + slideTime;
                    _timeSinceSlopeStarted = 0;
                    _slideTimerStarted = true;
                }

                if (isOnSlope && _rb.velocity.y < 0 && _slideTimerStarted){
                    _slideTimerStarted = false;
                }

                if (_slideTimerStarted && _slideTimer < Time.time){
                    sliding = false;
                }
            }
        }

        private void Update(){
            if (!isLocalPlayer) return;
            UpdateInputs();
            UpdateRotations();
            Jump();

            if (!_canJump){
                _rb.useGravity = true;
            }
            else
                _rb.useGravity = !isOnSlope;

            _currentCrouchHeight = Mathf.Lerp(_currentCrouchHeight, _goalCrouchHeight, crouchTransitionSpeed * Time.deltaTime);
            _colliderSize.y = _currentCrouchHeight;
            colliderTransform.localScale = _colliderSize;

            if (sliding && !_slideTimerStarted){
                _timeSinceSlopeStarted += Time.deltaTime;
            } 
        }

        private void CalculateAnimation(){
            //Convert global velocity to local relative to the players orientation
            Vector3 velocity = orientation.InverseTransformDirection(_rb.velocity / BaseMoveSpeed);
            velocity.y = 0;
            animator.SetFloat("X", velocity.x);
            animator.SetFloat("Y", velocity.z);

            handsAnimator.SetFloat("X", velocity.x);
            handsAnimator.SetFloat("Y", velocity.z);


            //Look IK
            rotation.rotation = Quaternion.Euler(_yMouse, _xMouse, 0);
            rotation.position = orientation.position + new Vector3(0, 1, 0) + rotation.forward * 5;

            //Adding player states
            animator.SetBool("Grounded", grounded);
            handsAnimator.SetBool("Grounded", grounded);

            animator.SetBool("Crouch", crouching);
            handsAnimator.SetBool("Crouch", crouching);
        }

        private float SlopeSpeed(){
            float angleMultiplier = (_angle - minSlopeAngle) / (maxSlopeAngle - minSlopeAngle);
            float goalSpeed = Mathf.Lerp(slideSpeed, maxSlideSpeed, angleMultiplier);
            float currentSpeed = Mathf.Lerp(slideSpeed, goalSpeed, _timeSinceSlopeStarted / 2.5f);
            return currentSpeed;
        }

        private void Move(){
            switch (isOnSlope){
                //Extra gravity
                case false:
                    _rb.AddForce(Vector3.down * 15, ForceMode.Acceleration);
                    break;
                //If on slope and moving down add force
                case true when _rb.velocity.y > 0:
                    _rb.AddForce(Vector3.down * 45, ForceMode.Acceleration);
                    break;
            }
          
            float currentAirControll = grounded ? 1 : airControll;
            Vector3 walkForce = GetDesiredDirection() * (CurrentMoveSpeed * currentAirControll);
            _rb.AddForce(walkForce, ForceMode.Acceleration);

            CounterMovement();
        }

        private void Jump(){
            //Makes it so the player can only jump on the ground
            if (!_canJump || !grounded || !Input.GetKeyDown(jump) || CursorManager.Singleton.WindowsOpend) return;

            Vector3 jumpNormal = Vector3.Lerp(Vector3.up, _groundNormal, mimicGroundAngle);

            _rb.AddForce(jumpNormal * jumpForce, ForceMode.VelocityChange);

            _groundNormal = Vector3.up;

            Invoke(nameof(ResetJump), jumpCooldown);

            _cameraController.SetPitch(-25);
        }

        private void ResetJump(){
            _canJump = true;
        }


        private void CounterMovement(){
            //General countermovement.
            Vector3 velocity = _rb.velocity;
            velocity.y = 0;

            if (!grounded) velocity = AdjustToAirControll(velocity);

            velocity *= -1;
            velocity = Vector3.ProjectOnPlane(velocity, _groundNormal);


            float currentAirControll = grounded ? 1 : airControll;

            Vector3 force = velocity * (counterMovementForce * currentAirControll);

            if (!grounded)
                force = Vector3.ClampMagnitude(force, CurrentMoveSpeed * currentAirControll);

            _rb.AddForce(force, ForceMode.Acceleration);

            //Harder stops if velocity is low to stop sliping.
            if (velocity.magnitude < 1f && grounded && !_startedWalking){
                _rb.AddForce(velocity * (counterMovementForce * 2), ForceMode.Acceleration);
            }
        }

        private Vector3 AdjustToAirControll(Vector3 velocity){
            //Stops player from getting friction in the air if no input is given.
            Vector3 adjustedVelocity = orientation.InverseTransformDirection(velocity);
            if (_xKeyboardRaw == 0) adjustedVelocity.x = 0;
            if (_yKeyboardRaw == 0) adjustedVelocity.z = 0;


            return orientation.TransformDirection(adjustedVelocity);
        }

        private Vector3 GetDesiredDirection(){
            //Gets the desired direction of the player by adding together inputs and look direction
            Vector3 direction = orientation.forward * _yKeyboard + orientation.right * _xKeyboard;
            direction = Vector3.ClampMagnitude(direction, 1);
            direction = Vector3.ProjectOnPlane(direction, _groundNormal);

            return direction;
        }

        private void UpdateInputs(){
            if (CursorManager.Singleton.WindowsOpend){
                return;
            }

            //Update all inputs
            _xMouse += Input.GetAxis("Mouse X") * xSensitivity * (invertX ? -1 : 1);
            _yMouse += Input.GetAxis("Mouse Y") * ySensitivity * (invertY ? -1 : 1);

            _yMouse = Mathf.Clamp(_yMouse, -85, 85);

            if (_gamePlayer.frozen || _gamePlayer.stun){
                _xKeyboard = 0;
                _xKeyboardRaw = 0;
                _yKeyboard = 0;
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

            if (useInterpolation){
                //If input interpolation is true then lerp to the action
                if (_xKeyboardRaw == 0)
                    _xKeyboard = 0;
                _xKeyboard = Mathf.Lerp(_xKeyboard, _xKeyboardRaw, interpolationSpeed * Time.deltaTime);

                if (_yKeyboardRaw == 0)
                    _yKeyboard = 0;
                _yKeyboard = Mathf.Lerp(_yKeyboard, _yKeyboardRaw, interpolationSpeed * Time.deltaTime);
            }
            else{
                _xKeyboard = _xKeyboardRaw;
                _yKeyboard = _yKeyboardRaw;
            }

            _startedWalking = _xKeyboardRaw != 0 || _yKeyboardRaw != 0;

            if (grounded && Input.GetKey(crouch) && !crouching){
                crouching = true;
                Vector3 velocity = _rb.velocity;
                velocity.y = 0;
                if (velocity.magnitude > BaseMoveSpeed / 2 && canSlide){
                    sliding = true;
                    _timeSinceSlopeStarted = 0;
                }
                _goalCrouchHeight = targetCrouchHeight;
                _gamePlayer.SetNameTagVisibility(false);
            }

            if (crouching && !grounded){
                crouching = false;
                sliding = false;
                _slideTimerStarted = false;
                _goalCrouchHeight = 1;
                _gamePlayer.SetNameTagVisibility(true);
            }

            if (!Input.GetKey(crouch) && crouching){
                crouching = false;
                sliding = false;
                _slideTimerStarted = false;
                _goalCrouchHeight = 1;
                _gamePlayer.SetNameTagVisibility(true);
            }
        }

        private void UpdateRotations(){
            orientation.rotation = Quaternion.Euler(0, _xMouse, 0);
            cameraHolder.rotation = Quaternion.Euler(_yMouse, _xMouse, 0);
        }


        private bool IsFloor(Vector3 v){
            float angle = Vector3.Angle(Vector3.up, v);
            _angle = angle;
            return angle < maxSlopeAngle;
        }

        private bool IsSlope(Vector3 v){
            float angle = Vector3.Angle(Vector3.up, v);
            return angle > minSlopeAngle;
        }

        private void OnCollisionStay(Collision other){
            if(!isLocalPlayer) return;
            //Make sure we are only checking for walkable layers
            int layer = other.gameObject.layer;
            if (groundLayers != (groundLayers | (1 << layer))) return;

            //Iterate through every collision in a physics update
            for (int i = 0; i < other.contactCount; i++){
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (!IsFloor(normal)) continue;
                isOnSlope = IsSlope(normal);
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
            isOnSlope = false;
            _groundNormal = Vector3.up;
        }
    }
}