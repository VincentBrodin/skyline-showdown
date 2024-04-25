using Code.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Code.Viewers{
    public class ViewerManager : NetworkBehaviour{
        [SyncVar] public int viewers;
        public int localGoal;
        public readonly UnityEvent<string, string> NewComment = new();
        public static ViewerManager Singleton{ get; private set; }
        
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private float _nextUpdate;

        private void Start(){
            if (isServer)
                viewers = Random.Range(9999, 999999);

            localGoal = viewers;
        }

        private void FixedUpdate(){
            if (viewers > localGoal) localGoal++;
            else if (viewers < localGoal) localGoal--;
            localGoal = Mathf.Clamp(localGoal, Mathf.Max(1000, viewers - 1000), viewers + 1000);
            if (!isServer) return;
            if (_nextUpdate > Time.time) return;
            _nextUpdate = Time.time + .5f;

            viewers += Random.Range(-250, 500);

            if (Random.Range(0, 5) == 0){
                SendMessage(ViewerData.Singleton.GetUser(), HandleMessage(ViewerData.Singleton.GetComment()));
            }
        }

        private void SendMessage(string user, string message){
            ClientSendMessage(user, message);
        }

        [ClientRpc]
        private void ClientSendMessage(string user, string message){
            NewComment.Invoke(user, message);
        }

        private string HandleMessage(string message){
            if (message.Contains("{player}")){
                message = message.Replace("{player}", $"{Manager().Players[Random.Range(0, Manager().Players.Count)].playerName}");
            }

            return message;
        }
    }
}