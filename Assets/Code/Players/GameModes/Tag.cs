using Code.Interface;
using Code.Networking;
using Code.Tools;
using Mirror;

namespace Code.Players.GameModes{
    public class Tag : NetworkBehaviour{
        [SyncVar] public bool tagged;
        private Punch _punch;
        private GamePlayer _gamePlayer;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }


        private void Start(){
            _punch = GetComponent<Punch>();
            _punch.OnHit.AddListener(OnHit);

            _gamePlayer = GetComponent<GamePlayer>();
        }


        private void OnHit(Punch.HitData hitData){
            if (_gamePlayer.gameMode != GameMode.Tag) return;
            if (!tagged) return;
            TagPlayer(hitData.VictimId);
        }

        private void TagPlayer(int player){
            foreach (GamePlayer gamePlayer in Manager().Players){
                if (gamePlayer.playerId != player) continue;
                if (gamePlayer.GetComponent<Tag>().tagged) return;
                break;
            }

            
            tagged = false;
            TaggedUi.Singleton.Hide();
            ServerTagPlayer(player);
        }

        [Command(requiresAuthority = false)]
        private void ServerTagPlayer(int player){
            foreach (GamePlayer gamePlayer in Manager().Players){
                if (gamePlayer.playerId != player) continue;
                gamePlayer.GetComponent<Tag>().ClientTagPlayer(player);
                break;
            }
        }

        [ClientRpc]
        private void ClientTagPlayer(int player){
            if (!isLocalPlayer) return;
            TaggedUi.Singleton.Show();
            tagged = true;
        }

        public void SetTagged(bool newValue){
            ServerSetTagged(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetTagged(bool newValue){
            ClientSetTagged(newValue);
        }

        [ClientRpc]
        private void ClientSetTagged(bool newValue){
            if (!isLocalPlayer) return;
            tagged = newValue;
            
            if(newValue)
                TaggedUi.Singleton.Show();
            else
                TaggedUi.Singleton.Hide();
        }
    }
}