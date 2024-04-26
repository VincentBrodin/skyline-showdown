using Code.Interface.Settings;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class CameraController : NetworkBehaviour{
        [Header("Head Bob")] [Range(0, 1)] public float headBobStrength;
        [Space] public Transform headBob;
        public float headBobSpeed = 14f;
        public float headBobAmount = 0.05f;
        [Space, Range(0, 1)] public float verticalStrength = 1;
        [Range(0, 1)] public float horizontalStrength = 1;

        [Header("Pitch")] 
        [Range(0, 1)] 
        public float pitchStrength;
        [Space] 
        public Transform pitch;
        public float inSpeed;
        public float outSpeed;

        [Header("References")] public Camera worldCamera;
        public Transform cameraHolder;
        public Transform cameraPosition;

        private Movement _movement;
        private Rigidbody _rb;
        private float _headBobTimer;
        private float _currentPitch, _goalPitch;

        private void Start(){
            _movement = GetComponent<Movement>();
            _rb = GetComponent<Rigidbody>();
            if (isLocalPlayer){
                cameraHolder.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else{
                cameraHolder.gameObject.SetActive(false);
            }

            SettingsMenu.Singleton.LoadingSettings.AddListener(LoadSettings);
            LoadSettings();
        }

        private void LateUpdate(){
            if (!isLocalPlayer) return;

            cameraHolder.position = cameraPosition.position;

            if (_movement.grounded){
                Vector3 velocity = _movement.orientation.InverseTransformDirection(_rb.velocity);

                float xPercent = (Mathf.Abs(velocity.x) / _movement.CurrentMaxMoveSpeed) * horizontalStrength;
                float yPercent = (Mathf.Abs(velocity.z) / _movement.CurrentMaxMoveSpeed) * verticalStrength;
                float percent = Mathf.Max(xPercent, yPercent);

                _headBobTimer += Time.deltaTime * headBobSpeed;

                headBob.localPosition = new Vector3(0,
                    (Mathf.Sin(_headBobTimer) * headBobAmount * percent) * headBobStrength, 0);
            }
            else{
                headBob.localPosition =
                    Vector3.Lerp(headBob.localPosition, Vector3.zero, 10 * Time.deltaTime);
            }

            _currentPitch = Mathf.Lerp(_currentPitch, _goalPitch, inSpeed * Time.deltaTime);
            _goalPitch = Mathf.Lerp(_goalPitch, 0, outSpeed * Time.deltaTime);
            pitch.localRotation = Quaternion.Euler(_currentPitch * pitchStrength, 0, 0);
        }

        public void SetPitch(float amount){
            _goalPitch += amount;
        }

        private void LoadSettings(){
            if (PlayerPrefs.HasKey("fov")){
                worldCamera.fieldOfView = (int)PlayerPrefs.GetFloat("fov");
            }
            else{
                worldCamera.fieldOfView = 70;
                PlayerPrefs.SetFloat("fov", worldCamera.fieldOfView);
            }

            if (PlayerPrefs.HasKey("head_bob")){
                headBobStrength = PlayerPrefs.GetFloat("head_bob");
            }
            else{
                headBobStrength = 1;
                PlayerPrefs.SetFloat("head_bob", headBobStrength);
            }
            
            if (PlayerPrefs.HasKey("head_pitch")){
                pitchStrength = PlayerPrefs.GetFloat("head_pitch");
            }
            else{
                pitchStrength = 1;
                PlayerPrefs.SetFloat("head_pitch", pitchStrength);
            }
        }
    }
}