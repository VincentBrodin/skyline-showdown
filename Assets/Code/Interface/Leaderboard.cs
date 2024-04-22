using System;
using System.Collections.Generic;
using System.Linq;
using Code.Networking;
using Code.Players;
using Mirror;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class Leaderboard : MonoBehaviour{
        public LeaderboardPosition[] leaderboardPositions;
        public float updateSpeed;
        private float _nextUpdate;

        private List<GamePlayer> _gamePlayers = new();
        
        [Serializable]
        public class LeaderboardPosition{
            public GameObject gameObject;
            public TextMeshProUGUI playerNameText;
            public TextMeshProUGUI playerScoreText;
        }

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Awake(){
            Manager().OnPlayerAdded.AddListener(OnChange);
            Manager().OnPlayerRemoved.AddListener(OnChange);
        }


        private void FixedUpdate(){
            if(_nextUpdate > Time.time) return;
            _nextUpdate = Time.time + updateSpeed;
            UpdateLeaderBoard();
        }

        private void OnChange(GamePlayer gamePlayer){
            UpdateLeaderBoard();
        }

        private void UpdateLeaderBoard(){
            _gamePlayers.Clear();
            Manager().Players.CopyTo(_gamePlayers);
            _gamePlayers = _gamePlayers.OrderBy(x => x.score).ToList();
            _gamePlayers.Reverse();

            for (int i = 0; i < leaderboardPositions.Length; i++){
                LeaderboardPosition leaderboardPosition = leaderboardPositions[i];
                if (i > _gamePlayers.Count-1){
                    leaderboardPosition.gameObject.SetActive(false);
                    continue;
                }

                GamePlayer gamePlayer = _gamePlayers[i];
                leaderboardPosition.playerNameText.text = gamePlayer.playerName;
                leaderboardPosition.playerScoreText.text = $"{gamePlayer.score}";
                leaderboardPosition.gameObject.SetActive(true);
            }
        }
    }
}