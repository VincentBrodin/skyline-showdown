using System;
using Mirror;
using UnityEngine;

namespace Code.Managers{
    public class Countdown : NetworkBehaviour{
        [SyncVar] public float time;
        [SyncVar] public string prompt;
        public bool ActiveCountdown => time > 0;
        public static Countdown Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }
        
        public void StartCountdown(float countdownLength, string countdownPrompt){
            ServerStartCountdown(countdownLength, countdownPrompt);
        }

        [Command(requiresAuthority = false)]
        private void ServerStartCountdown(float countdownLength, string countdownPrompt){
            time = countdownLength;
            prompt = countdownPrompt;
        }

        private void FixedUpdate(){
            if(!isServer) return;
            if(time <= 0) return;
            time -= Time.deltaTime;
        }
    }
}