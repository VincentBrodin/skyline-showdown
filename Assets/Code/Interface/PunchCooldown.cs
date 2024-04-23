using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class PunchCooldown : MonoBehaviour{
        public static PunchCooldown Singleton{ get; private set; }
        
        public Slider slider;
        public GameObject sliderObject;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void Set(float newValue){
            slider.value = newValue;

            sliderObject.SetActive(!Mathf.Approximately(newValue, 1f));
        }
    }
}