﻿using Code.Interface;
using Code.Managers;
using Code.Networking;
using Code.Tools;
using Mirror;
using UnityEngine;

namespace Code.Players.GameModes{
    public class KnockBack : NetworkBehaviour{
        [SyncVar] public float knockBackMultiplier;
        [SyncVar] public int hits;
        public int maxWarningHits;
        public float knockBackForce;
        private Punch _punch;
        private GamePlayer _gamePlayer;
        private Rigidbody _rb;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            _punch = GetComponent<Punch>();
            _punch.OnHit.AddListener(OnHit);

            _gamePlayer = GetComponent<GamePlayer>();
            _rb = GetComponent<Rigidbody>();

            if (isLocalPlayer)
                knockBackMultiplier = 1;
        }

        private void OnHit(Punch.HitData hitData){
            if (_gamePlayer.gameMode == GameMode.KingOfTheHill && _gamePlayer.gameActive)
                _gamePlayer.GiveScore(10, "HIT PLAYER");

            SetKnockBack(hitData.VictimId, knockBackForce, hitData.Direction);
        }

        private void SetKnockBack(int player, float force, Vector3 direction){
            ServerSetKnockBack(player, force, direction);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetKnockBack(int player, float force, Vector3 direction){
            foreach (GamePlayer gamePlayer in Manager().Players){
                if (gamePlayer.playerId != player) continue;
                gamePlayer.GetComponent<KnockBack>().ClientSetKnockBack(player, force, direction);
                break;
            }
        }

        [ClientRpc]
        private void ClientSetKnockBack(int player, float force, Vector3 direction){
            if (!isLocalPlayer) return;


            if (_gamePlayer.gameMode == GameMode.KingOfTheHill && _gamePlayer.gameActive){
                knockBackMultiplier *= 1.25f;
                hits += 1;
                Warning.Singleton.Set((float)hits / maxWarningHits);
                _rb.AddForce(
                    (direction * force + Vector3.up * 5) * knockBackMultiplier * _gamePlayer.metaData.knockBack,
                    ForceMode.VelocityChange);
                _gamePlayer.GiveScore(-5, $"GOT HIT");
                _gamePlayer.Stun();
            }
            else{
                _rb.AddForce((direction * force + Vector3.up * 10) * _gamePlayer.metaData.knockBack,
                    ForceMode.VelocityChange);
            }
        }

        public void SetMultiplier(float newValue){
            ServerSetMultiplier(newValue);
        }

        [Command(requiresAuthority = false)]
        private void ServerSetMultiplier(float newValue){
            ClientSetMultiplier(newValue);
        }

        [ClientRpc]
        private void ClientSetMultiplier(float newValue){
            if (!isLocalPlayer) return;
            knockBackMultiplier = newValue;
            hits = 0;
            Warning.Singleton.Set((float)hits / maxWarningHits);
        }
    }
}