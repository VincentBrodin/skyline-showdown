using System.Collections.Generic;
using Code.Networking;
using Mirror;
using UnityEngine;

namespace Code.MapTools{
    public class SpawnPoints : MonoBehaviour{
        public List<Transform> spawnPoints;
        public bool validate;
        public static SpawnPoints Singleton{ get; private set; }
        private bool _ready;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void FixedUpdate(){
            if (_ready || NetworkServer.isLoadingScene || NetworkClient.isLoadingScene) return;
            _ready = true;
            Manager().localPlayer.Teleport(spawnPoints[Manager().localPlayer.playerId].position);
        }


        protected void OnValidate(){
            for (int i = 0; i < transform.childCount; i++){
                if(!spawnPoints.Contains(transform.GetChild(i))) spawnPoints.Add(transform.GetChild(i));
            }
            
            for (int i = 0; i < spawnPoints.Count; i++){
                Transform spawnPointA = spawnPoints[i];
                for (int j = 0; j < spawnPoints.Count; j++){
                    if (i == j) continue;
                    Transform spawnPointB = spawnPoints[j];
                    if (spawnPointB == null){
                        spawnPoints[j] = Instantiate(spawnPointA, spawnPointB.parent);
                    }

                    if (spawnPointA == spawnPointB){
                        spawnPoints[j] = Instantiate(spawnPointA, spawnPointB.parent);
                    }
                }
            }

            for (int i = 0; i < spawnPoints.Count; i++){
                spawnPoints[i].name = $"Spawn Point: {i}";
                spawnPoints[i].SetSiblingIndex(i);
            }
            
           
        }

        private void OnDrawGizmos(){
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints){
                Gizmos.DrawSphere(spawnPoint.position, .5f);
            }
        }
    }
}