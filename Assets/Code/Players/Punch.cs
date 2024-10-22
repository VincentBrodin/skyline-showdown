﻿using System;
using Code.Interface;
using Code.Managers;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Players{
    public class Punch : NetworkBehaviour{
        public float range;
        public float radius;
        public float cooldown;
        public LayerMask layerMask;
        public Transform worldCamera;
        public int lastGotHitBy = -1;

        public readonly UnityEvent<HitData> OnHit = new();

        public struct HitData{
            public Vector3 From;
            public Vector3 To;
            public Vector3 Direction;
            public int AttackerId;
            public int VictimId;
        }

        private RaycastHit _hit;
        private float _nextPunch;

        private GamePlayer _gamePlayer;
        private CameraController _cameraController;

        private Ray _ray;

        private void Start(){
            _gamePlayer = GetComponent<GamePlayer>();
            _cameraController = GetComponent<CameraController>();
        }
        
        private void Update(){
            if (!isLocalPlayer) return;
            
            //Update cooldown
            float counter;
            if (_nextPunch < Time.time) counter = 1;
            else{
                float delta = _nextPunch - Time.time;
                counter = 1 - delta / cooldown;
            }
            
            PunchCooldown.Singleton.Set(counter);
            
            if(CursorManager.Singleton.WindowsOpend) return;
            if (!Input.GetKeyDown(KeyCode.Mouse0) || _nextPunch > Time.time) return;

            _nextPunch = Time.time + cooldown;
            _cameraController.SetPitch(15);
            _ray.origin = worldCamera.position;
            _ray.direction = worldCamera.forward;
            if (!Physics.SphereCast(_ray, radius, out _hit, range, layerMask)){
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

            gamePlayer.GetComponent<Punch>().GotHit(_gamePlayer.playerId);
        }
        
        private void GotHit(int byPlayer){
            ServerGotHit(byPlayer);
        }

        [Command(requiresAuthority = false)]
        private void ServerGotHit(int byPlayer){
            ClientGotHit(byPlayer);
        }

        [ClientRpc]
        private void ClientGotHit(int byPlayer){
            if(!isLocalPlayer) return;
            _cameraController.SetPitch(-15);
            lastGotHitBy = byPlayer;
        }
    }
}