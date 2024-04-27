using System.Collections.Generic;
using Code.Viewers;
using Mono.CSharp;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class CommentBoard : MonoBehaviour{
        public TextMeshProUGUI viewers;
        public Comment comment;
        public Transform commentParent;
        public RectTransform scrollArea;
        private readonly List<Comment> _comments = new();
        private float _goalHeight;
        private float _currentHeight;
        private float _baseHeight;

        private void Start(){
            ViewerManager.Singleton.NewComment.AddListener(NewComment);
            _goalHeight = scrollArea.sizeDelta.y;
            _currentHeight = _goalHeight;
            _baseHeight = _goalHeight;
        }

        private void FixedUpdate(){
            viewers.text = $"{ViewerManager.Singleton.localGoal}";

            _currentHeight = Mathf.Lerp(_currentHeight, _goalHeight, 3 * Time.fixedDeltaTime);
            scrollArea.sizeDelta = new Vector2(scrollArea.sizeDelta.x ,_currentHeight);
            scrollArea.anchoredPosition = new Vector2(0, _currentHeight / 2 - _baseHeight/2);
        }

        private void NewComment(string user, string message){
            Debug.Log($"{user} says: {message}");
            Comment newComment = Instantiate(comment, commentParent);
            newComment.user.text = user;
            newComment.message.text = message;
            _comments.Add(newComment);
            if (_comments.Count > 10){
                Comment c = _comments[0];
                Destroy(c.gameObject);
                _comments.RemoveAt(0);
            }
            if (_comments.Count > 4){
                _goalHeight += newComment.GetComponent<RectTransform>().sizeDelta.y + 0.125f;
            }
        }
    }
}