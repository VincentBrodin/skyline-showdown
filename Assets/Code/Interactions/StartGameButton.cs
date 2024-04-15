using Code.Managers;
using Code.Networking;
using Mirror;

namespace Code.Interactions{
    public class StartGameButton : NetworkBehaviour{
        public Selections map;
        public Interactive interactive;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            interactive.OnInteraction.AddListener(Load);
        }

        private void Load(){
            Invoke(nameof(ChangeScene), 5);
            Countdown.Singleton.StartCountdown(5, "STARTING IN:");
        }

        private void ChangeScene(){
            ServerChangeScene();
        }

        [Command(requiresAuthority = false)]
        private void ServerChangeScene(){
            Manager().ServerChangeScene(map.options[map.currentSelected].id);
        }
    }
}