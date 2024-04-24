using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Viewers{
    public class ViewerData : MonoBehaviour{
        public static ViewerData Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private CommentsData _data;

        [Serializable]
        private class CommentsData{
            public List<string> users = new();
            public List<string> comments = new();
        }

        private void Start(){
            string json = Resources.Load<TextAsset>("Comments").text;
            _data = JsonUtility.FromJson<CommentsData>(json);
        }

        public string GetUser(){
            return _data.users[Random.Range(0, _data.users.Count)];
        }

        public string GetComment(){
            return _data.comments[Random.Range(0, _data.comments.Count)];
        }
    }
}