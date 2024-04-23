using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class Warning : MonoBehaviour{
        public Color color;
        public Image image;
        public float endAlpha;
        public static Warning Singleton{ get; private set; }

        private float _alpha;

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

        public void Set(float percent){
            _alpha = Mathf.Lerp(0, endAlpha, percent);
            color.a = _alpha;
            image.color = color;
        }
    }
}