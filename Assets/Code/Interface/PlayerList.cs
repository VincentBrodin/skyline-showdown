using System;
using System.Collections.Generic;
using Code.Interface.Settings;
using Code.Networking;
using Code.Players;
using Mirror;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class PlayerList : MonoBehaviour{
        public KeyCode toggleKey = KeyCode.N;
        public GameObject show;
        public TextMeshProUGUI prompt;
        public bool showing;
        public static PlayerList Singleton;
        public TextMeshProUGUI playerText;
        public Transform parent;

        private float _nextNameUpdate;

        private readonly Dictionary<GamePlayer, TextMeshProUGUI> _texts = new();
        
        private CustomNetworkManager _manager;
        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        private void Start(){
            SettingsMenu.Singleton.LoadingSettings.AddListener(UpdatePrompt);
            show.SetActive(showing);

            foreach (GamePlayer gamePlayer in Manager().Players){
                AddPlayer(gamePlayer);
            }
        }

        public void AddPlayer(GamePlayer gamePlayer){
            if(_texts.ContainsKey(gamePlayer)) return;
            TextMeshProUGUI text = Instantiate(playerText, parent);
            text.text = gamePlayer.playerName;
            _texts.Add(gamePlayer, text);
        }

        public void RemovePlayer(GamePlayer gamePlayer){
            if(!_texts.TryGetValue(gamePlayer, out TextMeshProUGUI text)) return;
            Destroy(text.gameObject);
            _texts.Remove(gamePlayer);
        }

        private void FixedUpdate(){
            if(_nextNameUpdate > Time.time) return;
            _nextNameUpdate = Time.time + 1;

            foreach (KeyValuePair<GamePlayer, TextMeshProUGUI> textPair in _texts){
                textPair.Value.text = textPair.Key.playerName;
            }
        }

        private void Update(){
            if (!Input.GetKeyDown(toggleKey)) return;
            showing = !showing;
                
            show.SetActive(showing);
            UpdatePrompt();
        }

        private void UpdatePrompt(){
            prompt.text = showing ? $"HIDE PLAYER LIST - [{toggleKey}]" : $"SHOW PLAYER LIST - [{toggleKey}]";
        }
    }
}