using Code.Networking;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Interface{
    public class Preload : MonoBehaviour{
        private CustomNetworkManager _manager;
        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private bool _loaded;
        private void FixedUpdate(){
            if (!Manager().isActiveAndEnabled || _loaded) return;
            
            _loaded = true;
            SceneManager.LoadSceneAsync(Manager().offlineScene);
        }
    }
}