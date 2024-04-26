using System;
using Code.Interface.Settings;
using Code.Managers;
using Code.Networking;
using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class MainMenuInterface : MonoBehaviour{
        [Header("Main")] public Button playWithFriendsButton;
        public Button joinRandomButton;
        public Button optionButton;
        public Button creditsButton;
        public Button quitButton;

        [Header("Host")] public GameObject hostScreen;
        public Button hostGameButton;
        public Button joinLanGameButton;
        public Button backFromHostButton;
        public TMP_InputField ipAddress;
        public TMP_InputField port;
        public TMP_InputField nameInput;

        [Header("Error")] public GameObject errorScreen;
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

            if (!PlayerPrefs.HasKey("Name")){
                PlayerPrefs.SetString("Name", "Nerd");
            }
            else{
                nameInput.text = PlayerPrefs.GetString("Name");
            }
            
            nameInput.onValueChanged.AddListener(NameValueChanged);

            hostGameButton.onClick.AddListener(HostGame);
            joinLanGameButton.onClick.AddListener(JoinLanGame);
            backFromHostButton.onClick.AddListener(CloseHost);
            hostScreen.SetActive(false);

            Manager().transport.OnClientDisconnected += ClientError;

            closeError.onClick.AddListener(CloseError);
            errorScreen.SetActive(false);

            CursorManager.Singleton.ResetShow();

            ipAddress.text = Manager().networkAddress;
            port.text = $"{Manager().GetComponent<KcpTransport>().port}";
        }

        private void NameValueChanged(string newValue){
            PlayerPrefs.SetString("Name", newValue);
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
            if (!ushort.TryParse(port.text, out ushort newPort)) return;
            Manager().networkAddress = ipAddress.text;
            Manager().GetComponent<KcpTransport>().port = newPort;
            NetworkManager.singleton.StartHost();
        }

        private void JoinLanGame(){
            Debug.Log("Starting Lan");
            if (!ushort.TryParse(port.text, out ushort newPort)) return;
            Manager().networkAddress = ipAddress.text;
            Manager().GetComponent<KcpTransport>().port = newPort;
            NetworkManager.singleton.StartClient();
        }

        private void Quit(){
            Application.Quit();
        }

        private void ClientError(){
            if (errorScreen)
                errorScreen.SetActive(true);
        }

        private void OnDestroy(){
            if (Manager())
                Manager().transport.OnClientDisconnected += ClientError;
        }
    }
}