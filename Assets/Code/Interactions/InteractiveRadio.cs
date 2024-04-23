using Mirror;
using UnityEngine;

namespace Code.Interactions{
    public class InteractiveRadio : NetworkBehaviour{
        [SyncVar(hook = nameof(OnSwitch))] public int currentTrack;
        public AudioSource audioSource;
        public AudioClip[] tracks;
        public Interactive interactive;

        private void Start(){
            interactive.OnInteraction.AddListener(Switch);
        }

        private void Switch(){
            ServerSwitch();
        }

        [Command(requiresAuthority = false)]
        private void ServerSwitch(){
            if (currentTrack + 1 > tracks.Length - 1){
                currentTrack = 0;
            }
            else{
                currentTrack += 1;
            }
        }

        private void OnSwitch(int oldValue, int newValue){
            audioSource.Stop();
            audioSource.clip = tracks[newValue];
            audioSource.PlayDelayed(0.5f);
        }

        private void OnValidate(){
            if (interactive != null) return;
            Interactive onGameObject = GetComponent<Interactive>();
            Interactive onChild = GetComponentInChildren<Interactive>();

            if (onGameObject){
                interactive = onGameObject;
            }
            else if (onChild){
                interactive = onChild;
            }
        }
    }
}