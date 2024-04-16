using Code.Networking;
using Code.Tools;
using Mirror;
using UnityEngine;

namespace Code.Managers{
    public class LobbyManager : MonoBehaviour{
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private bool _ready;

        private void FixedUpdate(){
            if (_ready || NetworkServer.isLoadingScene || NetworkClient.isLoadingScene) return;
            _ready = true;
            Manager().localPlayer.gameMode = GameMode.None;
        }
    }
}