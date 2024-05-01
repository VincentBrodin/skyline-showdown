using Code.Networking;
using Code.Players;
using Mirror;
using UnityEngine;

namespace Code.Items{
    public class LandMine : NetworkBehaviour{
        [SyncVar] public bool rotating;

        [Header("Explosion settings")] public float radius;
        public float explosionForce;
        public LayerMask rayMask;

        [Header("Effects")] public float rotationSpeed;

        [Header("References")] public new Collider collider;
        public Rigidbody rb;
        public GameObject explosion;
        public Transform model;

        private Transform _transform;
        private float _rotation;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }


        private void Start(){
            _transform = transform;
            if (isServer)
                rotating = true;
            rb.isKinematic = false;
            collider.enabled = true;
        }

        private void LockIn(Vector3 position, Vector3 normal){
            ClientLockIn();
            rotating = false;
            _transform.position = position;
            _transform.up = normal;
            model.localRotation = Quaternion.Euler(Vector3.zero);
        }

        [ClientRpc]
        private void ClientLockIn(){
            collider.enabled = false;
            rb.isKinematic = true;
        }

        private void FixedUpdate(){
            if (!isServer) return;
            if (!rotating) return;
            rb.AddForce(Vector3.down * 15, ForceMode.Acceleration);
        }

        private void Update(){
            if (!isServer) return;
            if (!rotating) return;
            _rotation += rotationSpeed * Time.deltaTime;
            model.localRotation = Quaternion.Euler(_rotation, 0, 0);
        }

        private void OnCollisionEnter(Collision other){
            if (!isServer) return;
            if (!rotating) return;
            foreach (ContactPoint contact in other.contacts){
                if (!Physics.Raycast(_transform.position, contact.point - _transform.position, out RaycastHit hit,
                        10f, rayMask)) continue;
                LockIn(hit.point, hit.normal);
                break;
            }
        }

        public void Explode(){
            ClientExplode();
            foreach (GamePlayer player in Manager().Players){
                Vector3 playerPosition = player.Position() + new Vector3(0, 1, 0);
                Vector3 direction = playerPosition - _transform.position;
                float distanceToPlayer = direction.magnitude;
                bool rayHit = Physics.Raycast(_transform.position + _transform.up * .5f, direction, out RaycastHit hit,
                    radius);

                if (radius / 2 < distanceToPlayer){
                    if (!rayHit || hit.collider.transform.parent == null ||
                        hit.collider.transform.parent != player.transform) continue;
                }


                Vector3 directionToPlayer = direction.normalized;
                float percentOfForce = 1 - distanceToPlayer / radius;
                percentOfForce = Mathf.Clamp(percentOfForce, 0.5f, 1f);
                Vector3 force = directionToPlayer * (explosionForce * percentOfForce);

                if (radius / 2 > distanceToPlayer)
                    player.Stun();
                player.ResetFall();
                player.AddForce(force, ForceMode.VelocityChange);
            }

            Invoke(nameof(Kill), 4f);
        }

        private void Kill(){
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc]
        private void ClientExplode(){
            model.gameObject.SetActive(false);
            explosion.SetActive(true);
        }

        private void OnDrawGizmos(){
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.DrawRay(transform.position, transform.up);
        }
    }
}