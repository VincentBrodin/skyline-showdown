using Code.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Interface{
    public class ScreenCover : MonoBehaviour{
        public static ScreenCover Singleton{ get; private set; }

        public Image cover;
        public Color color;

        private float _from;
        private float _to;
        private float _time;
        private float _speed;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void Fade(float from, float to, float time){
            _from = from;
            _to = to;
           
            _speed = 1f / time;
            _time = 0;
            
            color.a = from;
            cover.color = color;
        }

        private void Update(){
            if(_time > 1) return;
            _time += _speed * Time.deltaTime;
            float t = _from > _to ? EasingFunctions.OutQuad(_time) : EasingFunctions.InQuad(_time);
            color.a = Mathf.Lerp(_from, _to, t);
            cover.color = color;
        }
    }
}