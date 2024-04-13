using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface.Settings
{
    public class AdjustableSetting : MonoBehaviour
    {
        public string saveKey;
        public CustomBehaviour customBehaviour;
        public bool isDropdown;
        public bool isKeybind;
      
        [Header("Slider")]
        public Slider slider;
        public TMP_InputField inputField;
        [Header("Dropdown")]
        public TMP_Dropdown dropDown;

        [Header("Keybind")] 
        public Button keybindButton;
        public TextMeshProUGUI keybindText;
        private bool _wating;

        private void Start(){
            switch (customBehaviour){
                case CustomBehaviour.Resolution:
                    dropDown.ClearOptions();
                    List<string> list = new();
                    foreach (Resolution resolution in Screen.resolutions){
                        list.Add($"{resolution.width} X {resolution.height} @ {Mathf.RoundToInt((float)resolution.refreshRateRatio.value)} Hz");
                    }
                    list.Reverse();
                    dropDown.AddOptions(list);
                    break;
            }
            
            if (isDropdown){
               dropDown.onValueChanged.AddListener(DropdownChanged);

               dropDown.value = PlayerPrefs.GetInt(saveKey, 0);
            }
            else if (isKeybind){
                keybindButton.onClick.AddListener(ChangeKey);
                keybindText.text = Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt(saveKey)).ToString();
            }
            else{
                slider.onValueChanged.AddListener(SliderChanged);
                inputField.onSubmit.AddListener(InputFieldChanged);
                inputField.onEndEdit.AddListener(InputFieldChanged);

                slider.value = PlayerPrefs.GetFloat(saveKey, slider.value);
                inputField.text = $"{Mathf.Round(slider.value * 10)/10}";
            }
        }

        private void ChangeKey(){
            keybindText.text = "...";
            _wating = true;
            SettingsMenu.Singleton.StartWating();
        }

        private void Update(){
            if (!_wating || !Input.anyKey) return;
            foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))){
                if(keycode == KeyCode.Escape) continue;
                if (!Input.GetKeyDown(keycode)) continue;
                _wating = false;
                keybindText.text = keycode.ToString();
                PlayerPrefs.SetInt(saveKey, (int)keycode);
                
                SettingsMenu.Singleton.LoadSettings();
                SettingsMenu.Singleton.StopWating();
            }
        }

        private void SliderChanged(float value){
            inputField.text = $"{Mathf.Round(value * 10)/10}";
            
            PlayerPrefs.SetFloat(saveKey, value);
            
            SettingsMenu.Singleton.LoadSettings();
        }

        private void InputFieldChanged(string value){
            if (!float.TryParse(value, out float floatValue)){
                inputField.text = $"{Mathf.Round(floatValue * 10)/10}";
            }

            slider.value = floatValue;
        }
        
        private void DropdownChanged(int option){
            PlayerPrefs.SetInt(saveKey, option);
            SettingsMenu.Singleton.LoadSettings();
        }
        
        public enum CustomBehaviour
        {
            None,
            Resolution
        }
    }
}