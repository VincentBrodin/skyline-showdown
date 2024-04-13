using System;
using Mirror;
using UnityEngine;

namespace Code.Networking{
    [RequireComponent(typeof(AudioSource))]
    public class NetworkAudio : NetworkBehaviour{
        public AudioSource audioSource;

        [SyncVar, Range(0, 1)] public float volume = 1;
        [SyncVar, Range(-3 ,3)] public float pitch = 1;

        private void Start(){
            if (!isServer) return;
            volume = audioSource.volume;
            pitch = audioSource.pitch;
        }

        public void Play(){
            CmdPlay();
        }
        
        [Command(requiresAuthority = false)]
        private void CmdPlay(){
            ClientPlay();
        }

        [ClientRpc]
        private void ClientPlay(){
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.Play();
        }
        
        public void PlayDelay(float delay){
            CmdPlayDelay(delay);
        }
        
        [Command(requiresAuthority = false)]
        private void CmdPlayDelay(float delay){
            ClientPlayDelay(delay);
        }
        
        [ClientRpc]
        private void ClientPlayDelay(float delay){
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.PlayDelayed(delay);
        }
    }
}