using System;
using System.Collections.Generic;
using Code.Interactions;
using Code.Managers;
using Code.Networking;
using Mirror;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class GameSettingsManager : NetworkBehaviour{
        public Selections mapSelection;
        public Selections gameModeSelection;
        public Selections overloadSelection;
        public int customIndex;

        private int _lastMap, _lastGameMode, _lastOverload;

        public List<GameSetting> gameSettingsList = new();
        public readonly Dictionary<string, GameSetting> GameSettings = new();

        private bool _ready;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }
        
        public static GameSettingsManager Singleton{ get; private set; }


        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        [Serializable]
        public class GameSetting{
            public string id;
            public GameObject gameObject;
            public TextMeshProUGUI text;
        }


        protected override void OnValidate(){
            base.OnValidate();
            foreach (GameSetting gameSetting in gameSettingsList){
                if (gameSetting.gameObject == null) continue;
                gameSetting.gameObject.name = gameSetting.id;
                gameSetting.text = gameSetting.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                gameSetting.text.text = $"{gameSetting.id}: ";
            }
        }

        private void Start(){
            foreach (GameSetting gameSetting in gameSettingsList){
                GameSettings.Add(gameSetting.id, gameSetting);
            }
        }

        private void FixedUpdate(){
            if (!_ready && Manager().localPlayer != null){
                _ready = true;
                UpdateScreen();
            }

            if (overloadSelection.currentSelected != _lastOverload)
                UpdateScreen();
            if (gameModeSelection.currentSelected != _lastGameMode)
                UpdateScreen();
            if (mapSelection.currentSelected != _lastMap)
                UpdateScreen();
        }

        private void UpdateScreen(){
            _lastOverload = overloadSelection.currentSelected;
            _lastMap = mapSelection.currentSelected;
            _lastGameMode = gameModeSelection.currentSelected;

            OverloadMetaData metaData =
                JsonUtility.FromJson<OverloadMetaData>(overloadSelection.options[_lastOverload].metaData);

            GameSettings["Overload"].text.text =
                $"OVERLOAD: {overloadSelection.options[_lastOverload].optionName}";
            GameSettings["Map"].text.text = $"MAP: {mapSelection.options[_lastMap].optionName}";
            GameSettings["GameMode"].text.text =
                $"GAME MODE: {gameModeSelection.options[_lastGameMode].optionName}";
            GameSettings["GameTime"].text.text = $"GAME TIME: {Mathf.Round(metaData.gameTime*10)/10}s";
            GameSettings["Speed"].text.text = $"PLAYER SPEED: {Mathf.Round(metaData.speed*10)/10}x";
            GameSettings["Gravity"].text.text = $"GRAVITY: {Mathf.Round(metaData.gravity*10)/10}x";
            GameSettings["Score"].text.text = $"SCORE: {Mathf.Round(metaData.score*10)/10}x";
            GameSettings["KnockBack"].text.text = $"KNOCK BACK: {Mathf.Round(metaData.knockBack*10)/10}x";



            if (_ready){
                Manager().localPlayer.metaData = metaData;
            }
        }

        public void SetCustomMeta(string metaData){
            overloadSelection.options[customIndex].metaData = metaData;
            UpdateScreen();
        }

        [Serializable]
        public class OverloadMetaData{
            public int gameTime;
            public float speed;
            public float gravity;
            public float score;
            public float knockBack;
        }
    }
}