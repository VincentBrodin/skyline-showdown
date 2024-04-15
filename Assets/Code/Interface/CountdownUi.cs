using System;
using Code.Managers;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class CountdownUi : MonoBehaviour{
        public TextMeshProUGUI prompt;
        public TextMeshProUGUI clock;
        public GameObject hideObject;

        private bool _displaying;

        private void Start(){
            _displaying = false;
            hideObject.SetActive(false);
        }

        private void FixedUpdate(){
            if (Countdown.Singleton.ActiveCountdown && !_displaying){
                hideObject.SetActive(true);
                _displaying = true;
                prompt.text = $"{Countdown.Singleton.prompt}";
                clock.text = TimeToString(Countdown.Singleton.time);
            }

            if (!Countdown.Singleton.ActiveCountdown && _displaying){
                hideObject.SetActive(false);
                _displaying = false;
            }

            if (_displaying){
                clock.text = TimeToString(Countdown.Singleton.time);
            }
        }

        private string TimeToString(float time){
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            float milliseconds = (time % 1) * 1000;
            milliseconds /= 10;
            milliseconds = Mathf.FloorToInt(milliseconds);
            string text = $"{minutes:00}:{seconds:00}";
            if (minutes == 0 && seconds < 10){
                text = $"{seconds:00}:{milliseconds:00}";
            }

            return text;
        }
    }
}