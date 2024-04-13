using Code.Networking;
using UnityEngine;

namespace Code.Interactions{
    public class InteractiveAudio : MonoBehaviour{
        public Interactive interactive;
        public NetworkAudio networkAudio;

        private void Start(){
            interactive.OnInteraction.AddListener(networkAudio.Play);
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