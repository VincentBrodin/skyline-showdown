using Code.Networking;
using Code.Tools;
using Mirror;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class GameModeUi : MonoBehaviour{
        public TextMeshProUGUI text;
        
        public static GameModeUi Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private bool _ready;
        
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void FixedUpdate(){
            if (_ready || Manager() == null || Manager().localPlayer == null) return;
            _ready = true;
            GameModeChanged(Manager().localPlayer.gameMode);
        }

        public void GameModeChanged(GameMode gameMode){
            text.text = $"GAME MODE: {gameMode}";
        }
    }
}