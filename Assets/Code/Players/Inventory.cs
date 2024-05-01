using System;
using System.Collections.Generic;
using Code.Interface;
using Code.Items;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class Inventory : NetworkBehaviour{
        public List<InventoryItem> items = new();
        public LandMine landMinePrefab;
        public Transform spawnPoint;

        private int _currentItem = 0;

        [Header("Key Bindings")] public KeyCode firstItem = KeyCode.Alpha1;
        public KeyCode secondItem = KeyCode.Alpha2;
        public KeyCode thirdItem = KeyCode.Alpha3;
        public KeyCode fourthItem = KeyCode.Alpha4;


        private LandMine _landMine;

        [Serializable]
        public class InventoryItem{
            public int uses;
            public bool hasItem;
            public float cooldown;
            [HideInInspector] public float cooldownEnd;
            [HideInInspector] public int currentUses;

        }

        private void Start(){
            InventoryUi.Singleton.Select(_currentItem);
        }

        private void FixedUpdate(){
            if (!isLocalPlayer) return;

            for (int i = 0; i < items.Count; i++){
                InventoryItem item = items[i];

                InventoryUi.Singleton.SetUnlocked(i, item.hasItem);
                InventoryUi.Singleton.SetUses(i, item.currentUses);


                if (item.cooldownEnd < Time.time){
                    InventoryUi.Singleton.SetCooldown(i, 0);
                    continue;
                }

                float timeLeft = item.cooldownEnd - Time.time;
                float percent = timeLeft / item.cooldown;
                InventoryUi.Singleton.SetCooldown(i, percent);
            }
        }

        private void Update(){
            if (!isLocalPlayer) return;

            if (Input.GetKeyDown(firstItem)){
                _currentItem = 0;
                InventoryUi.Singleton.Select(_currentItem);
            }

            if (Input.GetKeyDown(secondItem)){
                _currentItem = 1;
                InventoryUi.Singleton.Select(_currentItem);
            }

            if (Input.GetKeyDown(thirdItem)){
                _currentItem = 2;
                InventoryUi.Singleton.Select(_currentItem);
            }

            if (Input.GetKeyDown(fourthItem)){
                _currentItem = 3;
                InventoryUi.Singleton.Select(_currentItem);
            }


            if (!Input.GetKeyDown(KeyCode.Mouse1) || items[_currentItem].cooldownEnd > Time.time ||  !items[_currentItem].hasItem) return;

            if (_currentItem == 0){
                if (_landMine == null || _landMine.exploded){
                    SpawnLandMine(spawnPoint.position, spawnPoint.forward);
                }
                else{
                    ExplodeLandMine();
                    items[_currentItem].cooldownEnd = Time.time + items[_currentItem].cooldown;
                    items[_currentItem].currentUses--;
                    if (items[_currentItem].currentUses == 0)
                        items[_currentItem].hasItem = false;
                }
            }
            else{
                items[_currentItem].cooldownEnd = Time.time + items[_currentItem].cooldown;
                items[_currentItem].currentUses--;
                if (items[_currentItem].currentUses == 0)
                    items[_currentItem].hasItem = false;
            }
        }

        public void GiveItem(int index){
            items[index].hasItem = true;
            items[index].currentUses += items[index].uses;
        }

        [Command(requiresAuthority = false)]
        private void ExplodeLandMine(){
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