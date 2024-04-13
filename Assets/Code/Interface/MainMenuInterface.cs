using System;
using Code.Interface.Settings;
using Code.Managers;
using Code.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class MainMenuInterface : MonoBehaviour{
        [Header("Main")] 
        public Button playWithFriendsButton;
        public Button joinRandomButton;
        public Button optionButton;
        public Button creditsButton;
        public Button quitButton;

        [Header("Host")] 
        public GameObject hostScreen;
        public Button hostGameButton;
        public Button joinLanGameButton;
        public Button backFromHostButton;

        [Header("Error")] 
        public GameObject errorScreen;
        public Button closeError;

        private CustomNetworkManager _manager;
        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            playWithFriendsButton.onClick.AddListener(OpenHost);
            optionButton.onClick.AddListener(SettingsMenu.Singleton.Show);
            quitButton.onClick.AddListener(Quit);
            
            hostGameButton.onClick.AddListener(HostGame);
            joinLanGameButton.onClick.AddListener(JoinLanGame);
            backFromHostButton.onClick.AddListener(CloseHost);
            hostScreen.SetActive(false);

            Manager().transport.OnClientDisconnected += ClientError;
            
            closeError.onClick.AddListener(CloseError);
            errorScreen.SetActive(false);
            
            CursorManager.Singleton.ResetShow();
        }

        private void CloseError(){
            errorScreen.SetActive(false);
        }

        private void OpenHost(){
            hostScreen.SetActive(true);
            
        }

        private void CloseHost(){
            hostScreen.SetActive(false);
        }

        private void HostGame(){
            Debug.Log("Starting Host");
            NetworkManager.singleton.StartHost();
        }

        private void JoinLanGame(){
            Debug.Log("Starting Lan");
            NetworkManager.singleton.StartClient();
        }
        
        private void Quit(){
            Application.Quit();
        }

        private void ClientError(){
            if(errorScreen)
                errorScreen.SetActive(true);
        }

        private void OnDestroy(){
            if(Manager())
                Manager().transport.OnClientDisconnected += ClientError;
        }
    }
}