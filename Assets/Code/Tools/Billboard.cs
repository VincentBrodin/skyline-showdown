using Code.Networking;
using Code.Players;
using Mirror;
using UnityEngine;

namespace Code.Tools{
    public class Billboard : MonoBehaviour{
        private Transform _camera;
        private Transform _transform;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _transform = transform;
        }

        private void LateUpdate(){
            if (_camera == null){
                if (Manager().localPlayer)
                    _camera = Manager().localPlayer.GetComponent<CameraController>().cameraPosition;
                else
                    return;
            }

            _transform.forward = _transform.position - _camera.position;
        }
    }
}