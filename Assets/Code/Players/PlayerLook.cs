using Code.Networking;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class PlayerLook : NetworkBehaviour{
        public Transform look;
        public Vector3 offset;
        public Camera worldCamera;
        public float transitionSpeed;
        private GamePlayer _closestPlayer;
        private float _nextSearch;
        private Transform _worldCameraT;

        private Vector3 _currentPosition;
        private Vector3 _goalPosition;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _worldCameraT = worldCamera.transform;
        }

        private void FixedUpdate(){
            if (!isLocalPlayer) return;
            if (_nextSearch > Time.time) return;
            _nextSearch = Time.time + .5f;
            _closestPlayer = null;
            float closestDist = Mathf.Infinity;

            foreach (GamePlayer gamePlayer in Manager().Players){
                if (gamePlayer.isLocalPlayer) continue;
                Vector3 playerPosition = gamePlayer.Position() + offset;
                float dist = Vector3.Distance(playerPosition, _worldCameraT.position);

                if (IsOnScreen(playerPosition) && dist < closestDist){
                    closestDist = dist;
                    _closestPlayer = gamePlayer;
                }
            }
        }

        private void Update(){
            if (!isLocalPlayer) return;

            if (_closestPlayer == null){
                _goalPosition = _worldCameraT.position + _worldCameraT.forward * 3;
            }
            else
                _goalPosition = _closestPlayer.Position() + offset;

            _currentPosition = Vector3.Lerp(_currentPosition, _goalPosition, transitionSpeed * Time.deltaTime);
            look.position = _currentPosition;
        }

        private void OnDrawGizmos(){
            Gizmos.DrawWireSphere(look.position, .25f);
        }

        private bool IsOnScreen(Vector3 worldPoint){
            Vector3 directionToTarget = _worldCameraT.position - worldPoint;
            float angle = Vector3.Angle(_worldCameraT.forward, directionToTarget);
            return Mathf.Abs(angle) > 45;
        }
    }
}