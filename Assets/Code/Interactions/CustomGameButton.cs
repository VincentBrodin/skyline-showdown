using Code.Interface;
using UnityEngine;

namespace Code.Interactions{
    public class CustomGameButton : MonoBehaviour{
        public Interactive interactive;

        private void Start(){
            interactive.OnInteraction.AddListener(CustomGameUi.Singleton.Open);
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