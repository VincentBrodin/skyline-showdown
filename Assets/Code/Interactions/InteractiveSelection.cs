using System.Collections.Generic;
using UnityEngine;

namespace Code.Interactions{
    public class InteractiveSelection : MonoBehaviour{
        public Interactive interactive;

        public List<int> validIndex;
        public Selections selections;

        private void FixedUpdate(){
            if (validIndex.Contains(selections.currentSelected)){
                if (!interactive.active)
                    interactive.SetActive(true);
            }
            else{
                if (interactive.active)
                    interactive.SetActive(false);
            }
        }

        protected void OnValidate(){
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