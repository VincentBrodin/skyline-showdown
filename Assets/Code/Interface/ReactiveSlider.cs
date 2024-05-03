using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class ReactiveSlider : MonoBehaviour{
        public TMP_InputField inputField;
        public Slider slider;

        private void Start(){
            slider.onValueChanged.AddListener(SliderChanged);
            inputField.onSubmit.AddListener(InputFieldChanged);
            inputField.onEndEdit.AddListener(InputFieldChanged);
            
            inputField.text = $"{Mathf.Round(slider.value * 10)/10}";
        }
        
        private void SliderChanged(float value){
            inputField.text = $"{Mathf.Round(value * 10)/10}";
        }

        private void InputFieldChanged(string value){
            if (!float.TryParse(value, out float floatValue)){
                inputField.text = $"{Mathf.Round(floatValue * 10)/10}";
            }

            slider.value = floatValue;
        }
    }
}