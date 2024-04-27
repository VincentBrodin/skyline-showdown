using System;
using Code.Networking;
using Code.Players;
using Code.Players.GameModes;
using Code.Tools;
using Mirror;
using UnityEngine;

namespace Code.MapTools{
    public class AutoTp : NetworkBehaviour{
        public bool losePointsOnTp;
        public int amount = -25;
        public string prompt = "OPPS";
        public bool playAudioOnTp;
        public new NetworkAudio audio;
        [Space] public bool playParticles;
        public new ParticleSystem particleSystem;

        private Transform _particleSystem;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            if(particleSystem != null)
                _particleSystem = particleSystem.transform;
        }


        private void OnCollisionEnter(Collision other){
            if (!other.collider.transform.parent) return;
            if (!other.collider.transform.parent.TryGetComponent(out GamePlayer gamePlayer)) return;
            if (!gamePlayer.isLocalPlayer) return;
            
            if (playAudioOnTp && !audio.audioSource.isPlaying){
                audio.Play();
            }

            if (playParticles){
                PlayParticles(gamePlayer.Position());
            }

            gamePlayer.Teleport(SpawnPoints.Singleton.spawnPoints[gamePlayer.playerId].position);
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

        private void PlayParticles(Vector3 atPosition){
            ServerPlayParticles(atPosition);
        }

        [Command(requiresAuthority = false)]
        private void ServerPlayParticles(Vector3 atPosition){
            ClientPlayParticles(atPosition);
        }

        [ClientRpc]
        private void ClientPlayParticles(Vector3 atPosition){
            _particleSystem.position = atPosition;
            particleSystem.Play();
        }
    }
}