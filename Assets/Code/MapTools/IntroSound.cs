using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.MapTools{
    public class IntroSound : MonoBehaviour{
        public static IntroSound Singleton{ get; private set; }
        public AudioSource audioSource;
        public AudioClip[] clips;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private void Start(){
            audioSource.Stop();
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }
}