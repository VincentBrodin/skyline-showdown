using Code.Interface.Settings;
using Code.Managers;
using Code.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class EscapeMenu : MonoBehaviour
    {
        public bool showing;
        public Button continueButton;
        public Button settingsButton;
        public Button quitButton;
        public GameObject escapeMenuItem;
        
        private CustomNetworkManager _manager;
        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            continueButton.onClick.AddListener(Hide);
            settingsButton.onClick.AddListener(SettingsMenu.Singleton.Show);
            quitButton.onClick.AddListener(Quit);

            escapeMenuItem.SetActive(showing);
        }

        private void Quit(){
            Manager().localPlayer.Kick();
            
            if (Manager().localPlayer.isServer){
                Manager().StopHost();
            }
            else{
                Manager().StopClient();
            }
        }

        private void Update(){
            if (Input.GetKeyDown(KeyCode.Escape) && !SettingsMenu.Singleton.isOpen){
                if (showing){
                    Hide();
                }
                else{
                    showing = true;
                    CursorManager.Singleton.OpenWindow();
                    escapeMenuItem.SetActive(showing);
                }
            }
        }

        private void Hide(){
            showing = false;
            CursorManager.Singleton.CloseWindow();
            escapeMenuItem.SetActive(showing);
        }
    }
}