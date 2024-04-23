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

        private void Start(){
            Set(0);
        }


        private void Update(){
            _timer += Time.deltaTime * flashSpeed;
            float sin = 0.5f * (1 + Mathf.Sin(2 * Mathf.PI * _timer));
            color.a = sin * _alpha;
            image.color = color;
        }

        public void Set(float percent){
            _alpha = Mathf.Lerp(0, endAlpha, percent);
        }
    }
}