using Mirror;
using UnityEngine;

namespace Code.Interactions{
    public class InteractiveButton : NetworkBehaviour{
        public Interactive interactive;
        public Animator animator;
        public string triggerAnimation;

        private void Start(){
            interactive.OnInteraction.AddListener(OnInteraction);
        }

        private void OnInteraction(){
            TriggerOnServer();
        }

        [Command(requiresAuthority = false)]
        private void TriggerOnServer(){
            TriggerOnClients();
        }

        [ClientRpc]
        private void TriggerOnClients(){
            animator.SetTrigger(triggerAnimation);
        }
        
        protected override void OnValidate(){
            base.OnValidate();
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