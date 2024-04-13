using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class InteractionPrompt : MonoBehaviour{
        public static InteractionPrompt Singleton{ get; private set; }

        public GameObject hideObject;
        public TextMeshProUGUI prompt;


        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void Show(KeyCode key, string stringPrompt = "Interact"){
            prompt.text = $"{stringPrompt} - [{key}]";
            hideObject.SetActive(true);
        }

        public void Hide(){
            hideObject.SetActive(false);
        }
    }
}