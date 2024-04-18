using Code.Interface;
using Code.Networking;
using Code.Players;
using Code.Tools;
using Mirror;

namespace Code.Managers{
    public class LobbyManager : NetworkBehaviour{
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private bool _ready;

        private void Start(){
            ScreenCover.Singleton.FadeOut();
        }

        private void FixedUpdate(){
            if (_ready || NetworkServer.isLoadingScene || NetworkClient.isLoadingScene) return;
            _ready = true;
            Manager().localPlayer.gameMode = GameMode.None;
            if (isServer){
                UnFreezePlayers();
            }
        }
        
        private void FreezePlayers(){
            foreach (GamePlayer player in Manager().Players){
                player.Freeze();
            }
        }
        
  
        
        private void UnFreezePlayers(){
            foreach (GamePlayer player in Manager().Players){
                player.UnFreeze();
            }
        }
    }
}