using Code.Interface;
using Code.Networking;
using Code.Players;
using Mirror;

namespace Code.Managers{
    public class GameManager : NetworkBehaviour{

        private bool _ready;
        
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            ScreenCover.Singleton.FadeOut();
            CursorManager.Singleton.ResetHide();
        }

        private void FixedUpdate(){
            if(!isServer) return;
            if (_ready || NetworkServer.isLoadingScene || NetworkClient.isLoadingScene) return;
            _ready = true;
            Countdown.Singleton.StartCountdown(3, "STARTING IN:");
            FreezePlayers();
            Invoke(nameof(UnFreezePlayers), 3);
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