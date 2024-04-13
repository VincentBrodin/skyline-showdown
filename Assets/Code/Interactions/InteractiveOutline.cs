using UnityEngine;

namespace Code.Interactions{
    public class InteractiveOutline : MonoBehaviour{
        public Interactive interactive;
        public Outline outline;

        private void Start(){
            interactive.OnStartLookAt.AddListener(StartLooking);
            interactive.OnStopLookAt.AddListener(StopLooking);
            
            outline.enabled = false;
        }

        private void StartLooking(){
            outline.enabled = true;
        }

        private void StopLooking(){
            outline.enabled = false;
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