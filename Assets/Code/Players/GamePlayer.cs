using Code.Interface;
using Code.Managers;
using Code.Networking;
using Code.Tools;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class GamePlayer : NetworkBehaviour{
        [SyncVar] public int connectionId;
        [SyncVar] public int playerId;
        [SyncVar] public string playerName;
        [SyncVar] public bool frozen;
        [SyncVar(hook = nameof(GameModeChanged))] public GameMode gameMode = GameMode.None;

        private Rigidbody _rb;
        
        private CustomNetworkManager _manager;
        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _rb = GetComponent<Rigidbody>();
            
            if (isLocalPlayer){
                //Update player values
                playerName = $"Nerd {playerId}";
                Manager().localPlayer = this;
                    
                //Set random spawn point
                _rb.position = new Vector3(Random.Range(-1.25f, 1.25f), 0, Random.Range(-1.25f, 1.25f));
                
                //Reset cursor
                CursorManager.Singleton.ResetHide();
            }
            
            Manager().AddPlayer(this);
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy(){
            (NetworkManager.singleton as CustomNetworkManager)?.RemovePlayer(this);
        }

        public override void OnStopClient(){
            Manager().RemovePlayer(this);
            base.OnStopClient();
        }


        public void Kick(){
            DestroyImmediate(GetComponent<CameraController>().cameraHolder.gameObject);
        }

        public void Teleport(Vector3 teleportTo){
            ServerTeleport(teleportTo);
        }

        [Command(requiresAuthority = false)]
        private void ServerTeleport(Vector3 teleportTo){
            ClientTeleport(teleportTo);
        }

        [ClientRpc]
        private void ClientTeleport(Vector3 teleportTo){
            Debug.Log($"Teleporting {playerName} from {_rb.position} to {teleportTo}");
            if(!isLocalPlayer) return;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.position = teleportTo;
            transform.position = teleportTo;
        }

        public void Freeze(){
            ServerFreeze();
        }

        [Command(requiresAuthority = false)]
        private void ServerFreeze(){
            frozen = true;
            ClientFreeze();
        }

        [ClientRpc]
        private void ClientFreeze(){
            if (isLocalPlayer){
                frozen = true;
            }
        }
        
        public void UnFreeze(){
            ServerUnFreeze();
        }

        [Command(requiresAuthority = false)]
        private void ServerUnFreeze(){
            frozen = false;
            ClientUnFreeze();
        }
        
        [ClientRpc]
        private void ClientUnFreeze(){
            if (isLocalPlayer){
                frozen = false;
            }
        }

        private void GameModeChanged(GameMode oldValue, GameMode newValue){
            if(!isLocalPlayer) return;
            GameModeUi.Singleton.GameModeChanged(newValue);
        }
    }
}