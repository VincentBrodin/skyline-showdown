using Code.Managers;
using Code.Networking;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class GamePlayer : NetworkBehaviour{
        [SyncVar] public int connectionId;
        [SyncVar] public int playerId;
        [SyncVar] public string playerName;

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
    }
}