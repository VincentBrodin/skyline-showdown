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
            ChangeScene();
        }

        [Command(requiresAuthority = false)]
        private void ChangeScene(){
            Manager().ServerChangeScene(map.options[map.currentSelected].id);
        }
    }
}