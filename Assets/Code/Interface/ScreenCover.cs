using UnityEngine;

namespace Code.Interface{
    public class ScreenCover : MonoBehaviour{
        public static ScreenCover Singleton{ get; private set; }

        public GameObject fadeIn;
        public GameObject fadeOut;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void FadeIn(){
            fadeIn.SetActive(true);
            fadeOut.SetActive(false);
        }

        public void FadeOut(){
            fadeOut.SetActive(true);
            fadeIn.SetActive(false);
        }

    
    }
}