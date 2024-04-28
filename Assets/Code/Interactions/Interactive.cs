using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Interactions{
    public class Interactive : NetworkBehaviour{
        [SyncVar(hook = nameof(ActiveChanged))] public bool active = true;
        [SerializeField] private string activePrompt;
        [SerializeField] private string inactivePrompt;
        
        public readonly UnityEvent OnInteraction = new ();
        public readonly UnityEvent OnStartLookAt = new ();
        public readonly UnityEvent OnStopLookAt = new ();
        public readonly UnityEvent<bool> OnActiveChanged = new();

        public string Prompt => active ? activePrompt : inactivePrompt;

        public void Interact(){
            OnInteraction.Invoke();
        }

        public void LookAt(){
            OnStartLookAt.Invoke();
        }

        public void StopLooking(){
            OnStopLookAt.Invoke();
        }

        private void ActiveChanged(bool oldValue, bool newValue){
            OnActiveChanged.Invoke(newValue);
        }

        public void SetActive(bool newValue){
            CmdSetActive(newValue);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetActive(bool newValue){
            active = newValue;
        }

        public void SetPrompt(string newPrompt, bool changeActivePrompt = true){
            if (changeActivePrompt){
                activePrompt = newPrompt;
            }
            else{
                inactivePrompt = newPrompt;
            }
        }
    }
}