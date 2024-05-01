using System;
using System.Collections.Generic;
using Code.Items;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class Inventory : NetworkBehaviour{
        public LandMine landMinePrefab;
        public Transform spawnPoint;
        public float landMineThrowDelay;
        private LandMine _landMine;
        private bool _hasLandMine;

        private float _lastThrownLandMine;

        private void FixedUpdate(){
            if (!isServer) return;

            if (_landMine == null)
                _hasLandMine = false;
        }

        private void Update(){
            if(!isLocalPlayer) return;
            
            if (Input.GetKeyDown(KeyCode.Mouse1) && !_hasLandMine && _lastThrownLandMine < Time.time){
                SpawnLandMine(spawnPoint.position, spawnPoint.forward);
                _lastThrownLandMine = Time.time + landMineThrowDelay;
                _hasLandMine = true;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && _hasLandMine){
                ExplodeLandMines();
                _hasLandMine = false;
            }
        }

        [Command(requiresAuthority = false)]
        
        private void ExplodeLandMines(){
            _landMine.Explode();
        }

        [Command(requiresAuthority = false)]
        private void SpawnLandMine(Vector3 spawnAt, Vector3 direction){
            LandMine landMine = Instantiate(landMinePrefab, spawnAt, Quaternion.LookRotation(direction));
            NetworkServer.Spawn(landMine.gameObject);
            _landMine = landMine;
            landMine.rb.AddForce(direction * 20f, ForceMode.VelocityChange);
        }
    }
}