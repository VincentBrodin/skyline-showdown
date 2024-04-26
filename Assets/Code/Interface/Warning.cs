using Code.Interface.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class Warning : MonoBehaviour{
        public Color color;
        public Image image;
        public float endAlpha;
        public float flashSpeed;
        public static Warning Singleton{ get; private set; }

        private float _alpha;
        private float _timer;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private bool _showBlink;

        private void Start(){
            Set(0);
            
            SettingsMenu.Singleton.LoadingSettings.AddListener(OnSettingsLoad);
            OnSettingsLoad();
        }

        private void OnSettingsLoad(){
            _showBlink = PlayerPrefs.GetInt("warning_blink") == 1;
        }
        
        


        private void Update(){
            if(!_showBlink) return;
            _timer += Time.deltaTime * flashSpeed;
            float sin = 0.5f * (1 + Mathf.Sin(2 * Mathf.PI * _timer));
            color.a = sin * _alpha;
            image.color = color;
        }

        public void Set(float percent){
            if(!_showBlink) return;
            _alpha = Mathf.Lerp(0, endAlpha, percent);
        }
    }
}