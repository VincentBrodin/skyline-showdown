using UnityEngine;

namespace Code.Interactions{
    public class InteractiveMaterialChange : MonoBehaviour{
        public Interactive interactive;
        public MeshRenderer meshRenderer;
        public Material active;
        public Material inActive;

        private void Start(){
            interactive.OnActiveChanged.AddListener(UpdateMaterial);
            
            UpdateMaterial(interactive.active);
        }

        private void UpdateMaterial(bool newValue){
            meshRenderer.material = newValue ? active : inActive;
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