using System;
using Code.Networking;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.MapTools{
    public class IntroSound : MonoBehaviour{
        public static IntroSound Singleton{ get; private set; }
        public AudioSource audioSource;
        public AudioClip[] clips;
        public AudioClip lockIn;

        private bool _ready;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }
        

        public void PlayLockIn(){
            audioSource.Stop();
            audioSource.clip = lockIn;
            audioSource.Play();
        }

        private void FixedUpdate(){
            if (_ready || !Manager().localPlayer) return;
            _ready = true;
            if (!Manager().localPlayer.firstTimeInLobby) return;
            Manager().localPlayer.firstTimeInLobby = false;
            audioSource.Stop();
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }
}