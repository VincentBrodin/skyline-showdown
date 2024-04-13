using UnityEngine;

namespace Code.Interactions{
    public class InteractiveToggle : MonoBehaviour{
        public bool valueToSet;
        public Interactive interactive;
        public Interactive[] interactivesToToggle;

        private void Start(){
            interactive.OnInteraction.AddListener(Toggle);
        }

        public void Toggle(){
            foreach (Interactive interactiveToToggle in interactivesToToggle){
                interactiveToToggle.SetActive(valueToSet);
            }
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