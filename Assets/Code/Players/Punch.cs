using Code.Managers;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Players{
    public class Punch : NetworkBehaviour{
        public float range;
        public float cooldown;
        public LayerMask layerMask;
        public Transform worldCamera;

        public readonly UnityEvent<HitData> OnHit = new();

        public struct HitData{
            public Vector3 From;
            public Vector3 To;
            public Vector3 Direction;
            public int AttackerId;
            public int VictimId;
        }

        private RaycastHit _hit;
        private bool _canPunch;

        private GamePlayer _gamePlayer;

        private void Start(){
            _gamePlayer = GetComponent<GamePlayer>();
            _canPunch = true;
        }

        private void Update(){
            if (!isLocalPlayer) return;
            if(CursorManager.Singleton.WindowsOpend) return;
            if (!Input.GetKeyDown(KeyCode.Mouse0) || !_canPunch) return;
            
            _canPunch = false;
            Invoke(nameof(ResetPunch), cooldown);

            if (!Physics.Raycast(worldCamera.position, worldCamera.forward, out _hit, range, layerMask)){
                Debug.Log("Punch Miss");
                return;
            }
            
            Debug.Log("Punch Hit");

            if (_hit.collider.transform.parent == null ||
                !_hit.collider.transform.parent.TryGetComponent(out GamePlayer gamePlayer)){
                return;
            }
            
            OnHit.Invoke(new HitData{
                From = worldCamera.position,
                To = _hit.point,
                Direction = (_hit.point - worldCamera.position).normalized,
                AttackerId = _gamePlayer.playerId,
                VictimId = gamePlayer.playerId
            });
        }

        private void ResetPunch(){
            _canPunch = true;
        }
    }
}