using Code.Interface;
using Code.Managers;
using Code.Networking;
using Code.Tools;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Players{
    public class GamePlayer : NetworkBehaviour{
        [SyncVar] public int connectionId;
        [SyncVar] public int playerId;
        [SyncVar] public string playerName;
        [SyncVar] public bool frozen;
        [SyncVar] public int score;
        [SyncVar(hook = nameof(GameModeChanged))]
        public GameMode gameMode = GameMode.None;
        [Space]
        [SerializeField] private int personalLayer;

        [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
        private Rigidbody _rb;
        private Transform _transform;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _rb = GetComponent<Rigidbody>();
            _transform = transform;

            if (isLocalPlayer){
                //Update player values
                playerName = $"Nerd {playerId}";
                Manager().localPlayer = this;

                //Set random spawn point
                _rb.position = new Vector3(Random.Range(-1.25f, 1.25f), 0, Random.Range(-1.25f, 1.25f));

                //Reset cursor
                CursorManager.Singleton.ResetHide();
                
                gameObject.SetLayer(personalLayer, true);

                foreach (SkinnedMeshRenderer meshRenderer in meshRenderers){
                    meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }

            Manager().AddPlayer(this);

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy(){
            if(GetComponent<CameraController>() && GetComponent<CameraController>().cameraHolder)
                Destroy(GetComponent<CameraController>().cameraHolder.gameObject);
            (NetworkManager.singleton as CustomNetworkManager)?.RemovePlayer(this);
        }

        public override void OnStopClient(){
            if(GetComponent<CameraController>() && GetComponent<CameraController>().cameraHolder)
                Destroy(GetComponent<CameraController>().cameraHolder.gameObject);
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
            if (!isLocalPlayer) return;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.position = teleportTo;
            _transform.position = teleportTo;
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
            if (!isLocalPlayer) return;
            frozen = true;
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
            if (!isLocalPlayer) return;
            frozen = false;
        }

        private void GameModeChanged(GameMode oldValue, GameMode newValue){
            if (!isLocalPlayer) return;
            GameModeUi.Singleton.GameModeChanged(newValue);
        }

        public void GiveScore(int scoreToGive, string prompt){
            ServerGiveScore(scoreToGive, prompt);
        }

        [Command(requiresAuthority = false)]
        private void ServerGiveScore(int scoreToGive, string prompt){
            ClientGiveScore(scoreToGive, prompt);
        }

        [ClientRpc]
        private void ClientGiveScore(int scoreToGive, string prompt){
            if (!isLocalPlayer) return;
            if (score + scoreToGive < 0){
                score = 0;
            }
            else
                score += scoreToGive;
            ScoreUi.Singleton.UpdateScore(scoreToGive, prompt);
        }

        public Vector3 Position(){
            return _transform.position;
        }
       
    }
}