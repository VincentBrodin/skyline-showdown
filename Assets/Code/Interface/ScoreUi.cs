using System;
using System.Collections.Generic;
using Code.Networking;
using Mirror;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class ScoreUi : MonoBehaviour{
        public static ScoreUi Singleton{ get; private set; }
        public TextMeshProUGUI scoreText;
        public Transform scorePlusParent;
        public TextMeshProUGUI scorePlusPrefab;
        public List<TextMeshProUGUI> scorePluses;

        private CustomNetworkManager _manager;
        private int _currentScore;
        private int _scoreToGet;
        private float _nextUpdate;

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            if (Manager().localPlayer){
                UpdateScore();
                _currentScore = _scoreToGet;
                scoreText.text = $"SCORE: {_currentScore}";
            }
            else scoreText.text = $"SCORE: 0";

            _currentScore = 0;
            _scoreToGet = 0;

            for (int i = 0; i < 10; i++){
                TextMeshProUGUI scorePlus = Instantiate(scorePlusPrefab, scorePlusParent);
                scorePlus.gameObject.SetActive(false);
                scorePluses.Add(scorePlus);
            }
        }

        private void FixedUpdate(){
            if (_currentScore == _scoreToGet){
                scoreText.color = Color.white;
                return;
            }

            if (_nextUpdate > Time.time) return;
            _nextUpdate = Time.time + .05f;

            if (_currentScore < _scoreToGet){
                _currentScore++;
                scoreText.color = Color.white;
            }
            else{
                _currentScore--;
                scoreText.color = Color.red;
            }


            scoreText.text = $"SCORE: {_currentScore}";
        }

        public void UpdateScore(){
            _scoreToGet = Manager().localPlayer.score;
        }

        public void UpdateScore(int amount, string prompt){
            _scoreToGet = Manager().localPlayer.score;
            TextMeshProUGUI scorePlus = GetScorePlus();
            string type = amount > 0 ? "+" : "";
            scorePlus.color = amount > 0 ? Color.white : Color.red;
            scorePlus.text = $"{prompt} {type}{amount}";
        }

        private TextMeshProUGUI GetScorePlus(){
            foreach (TextMeshProUGUI scorePlus in scorePluses){
                if (scorePlus.gameObject.activeSelf) continue;
                scorePlus.gameObject.SetActive(true);
                return scorePlus;
            }

            TextMeshProUGUI newScorePlus = Instantiate(scorePlusPrefab, scorePlusParent);
            scorePluses.Add(newScorePlus);
            return newScorePlus;
        }
    }
}