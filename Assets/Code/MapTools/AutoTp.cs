using System;
using Code.Networking;
using Code.Players;
using Code.Players.GameModes;
using Code.Tools;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MapTools{
    public class AutoTp : NetworkBehaviour{
        public bool losePointsOnTp;
        public int amount = -25;
        public string prompt = "OPPS";
        public bool playAudioOnTp;
        public new NetworkAudio audio;
        [Space] public bool playParticles;
        public ParticleSystem dieParticles;
        public ParticleSystem spawnParticles;
        private Transform _spawnParticles;

        private Transform _dieParticle;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            if (dieParticles != null)
                _dieParticle = dieParticles.transform;

            if (spawnParticles != null)
                _spawnParticles = spawnParticles.transform;
        }


        private void OnCollisionEnter(Collision other){
            if (!other.collider.transform.parent) return;
            if (!other.collider.transform.parent.TryGetComponent(out GamePlayer gamePlayer)) return;
            if (!gamePlayer.isLocalPlayer) return;

            if (playAudioOnTp && !audio.audioSource.isPlaying){
                audio.Play();
            }

            Vector3 newPosition = SpawnPoints.Singleton.spawnPoints[gamePlayer.playerId].position;

            if (playParticles){
                PlayParticles(gamePlayer.Position(), newPosition);
            }

            gamePlayer.Teleport(newPosition);
            gamePlayer.GetComponent<KnockBack>().SetMultiplier(1);

            if (losePointsOnTp){
                gamePlayer.GiveScore(amount, prompt);
                if (gamePlayer.gameMode == GameMode.KingOfTheHill){
                    int id = gamePlayer.GetComponent<Punch>().lastGotHitBy;
                    if (id == -1) return;
                    foreach (GamePlayer player in Manager().Players){
                        if (player.playerId == id){
                            player.GiveScore(15, $"KNOCKED OF {gamePlayer.playerName}");
                        }
                    }

                    gamePlayer.GetComponent<Punch>().lastGotHitBy = -1;
                }
            }
        }

        private void PlayParticles(Vector3 atPosition, Vector3 toPosition){
            ServerPlayParticles(atPosition, toPosition);
        }

        [Command(requiresAuthority = false)]
        private void ServerPlayParticles(Vector3 atPosition, Vector3 toPosition){
            ClientPlayParticles(atPosition, toPosition);
        }

        [ClientRpc]
        private void ClientPlayParticles(Vector3 atPosition, Vector3 toPosition){
            _dieParticle.position = atPosition;
            dieParticles.Play();

            _spawnParticles.position = toPosition;
            spawnParticles.Play();
        }
    }
}