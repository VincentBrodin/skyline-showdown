using Code.Managers;
using Code.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class CustomGameUi : MonoBehaviour{
        public Slider gameTimeSlider;
        public Slider scoreSlider;
        public Slider knockBackSlider;
        public Slider gravitySlider;
        public Slider speedSlider;
        public GameObject hideObject;

        public bool isOpen;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }
        
        public static CustomGameUi Singleton{ get; private set; }


        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private void Start(){
            gameTimeSlider.onValueChanged.AddListener(ValueChanged);
            scoreSlider.onValueChanged.AddListener(ValueChanged);
            knockBackSlider.onValueChanged.AddListener(ValueChanged);
            gravitySlider.onValueChanged.AddListener(ValueChanged);
            speedSlider.onValueChanged.AddListener(ValueChanged);

            isOpen = false;
            hideObject.SetActive(false);
        }

        private void ValueChanged(float newValue){
            Manager().localPlayer.SyncMetaData(gameTimeSlider.value, scoreSlider.value, knockBackSlider.value,
                gravitySlider.value, speedSlider.value);
        }
        
      

        private void LateUpdate(){
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen){
                Close();
            }
        }

        public void Open(){
            gameTimeSlider.value = Manager().localPlayer.metaData.gameTime;
            scoreSlider.value = Manager().localPlayer.metaData.score;
            knockBackSlider.value = Manager().localPlayer.metaData.knockBack;
            gravitySlider.value = Manager().localPlayer.metaData.gravity;
            speedSlider.value = Manager().localPlayer.metaData.speed;

            
            hideObject.SetActive(true);
            CursorManager.Singleton.OpenWindow();
            isOpen = true;
        }

        public void Close(){
            hideObject.SetActive(false);
            CursorManager.Singleton.CloseWindow();
            isOpen = false;
        }
    }
}