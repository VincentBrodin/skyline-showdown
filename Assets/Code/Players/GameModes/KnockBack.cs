using Code.Networking;
using Code.Tools;
using Mirror;
using UnityEngine;

namespace Code.Players.GameModes{
    public class KnockBack : NetworkBehaviour{
        [SyncVar]public float knockBackMultiplier;
        public float knockBackForce;
        private Punch _punch;
        private GamePlayer _gamePlayer;
        private Rigidbody _rb;
        
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
            _rb = GetComponent<Rigidbody>();

            if (isLocalPlayer)
                knockBackMultiplier = 1;
        }
        
        private void OnHit(Punch.HitData hitData){
            if (_gamePlayer.gameMode == GameMode.KingOfTheHill)
                _gamePlayer.GiveScore(10, "HIT PLAYER");
            
            SetKnockBack(hitData.VictimId, knockBackForce, hitData.Direction);
        }

        private void SetKnockBack(int player, float force, Vector3 direction){
            ServerSetKnockBack(player, force, direction);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetKnockBack(int player, float force, Vector3 direction){
            foreach (GamePlayer gamePlayer in Manager().Players){
                if(gamePlayer.playerId != player) continue;
                gamePlayer.GetComponent<KnockBack>().ClientSetKnockBack(player, force, direction);
                break;
            }
        }

        [ClientRpc]
        private void ClientSetKnockBack(int player, float force, Vector3 direction){
            if(!isLocalPlayer) return;
            if (_gamePlayer.gameMode == GameMode.KingOfTheHill){
                knockBackMultiplier *= 1.25f;
                _rb.AddForce(direction * force * knockBackMultiplier, ForceMode.VelocityChange);
                _gamePlayer.GiveScore(-5, "GOT HIT");
            }
            else{
                _rb.AddForce(direction * force, ForceMode.VelocityChange);
            }
        }

        public void SetMultiplier(float newValue){
            ServerSetMultiplier(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetMultiplier(float newValue){
            ClientSetMultiplier(newValue);
        }

        [ClientRpc]
        private void ClientSetMultiplier(float newValue){
            if(!isLocalPlayer) return;
            knockBackMultiplier = newValue;
        }
    }
}