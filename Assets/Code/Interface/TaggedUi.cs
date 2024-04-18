using UnityEngine;

namespace Code.Interface{
    public class TaggedUi : MonoBehaviour{

        public GameObject tagObject;
        public static TaggedUi Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void Show(){
            tagObject.SetActive(true);
        }

        public void Hide(){
            tagObject.SetActive(false);
        }
    }
}