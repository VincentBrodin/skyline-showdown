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

        /// <summary>
        /// Syncs the Play function from AudioSource to all clients
        /// </summary>
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

        /// <summary>
        /// Syncs the Stop function from AudioSource to all clients
        /// </summary>
        public void Stop(){
            CmdStop();
        }

        [Command(requiresAuthority = false)]
        private void CmdStop(){
            ClientStop();
        }

        [ClientRpc]
        private void ClientStop(){
            audioSource.Stop();
        }
        
        /// <summary>
        /// Syncs the PlayDelay function from AudioSource to all clients
        /// </summary>
        /// <param name="delay">The given delay</param>
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
        
        /// <summary>
        /// Syncs the volume between clients
        /// </summary>
        /// <param name="newValue">The new volume (0,1)</param>
        public void SetVolume(float newValue){
            CmdSetVolume(newValue);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetVolume(float newValue){
            newValue = Mathf.Clamp(newValue, 0, 1);
            volume = newValue;
        }
        
        /// <summary>
        /// Syncs the pitch between clients
        /// </summary>
        /// <param name="newValue">The new pitch (-3,3)</param>
        public void SetPitch(float newValue){
            CmdSetPitch(newValue);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetPitch(float newValue){
            newValue = Mathf.Clamp(newValue, -3, 3);
            pitch = newValue;
        }
    }
}