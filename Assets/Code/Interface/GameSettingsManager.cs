using System;
using System.Collections.Generic;
using Code.Interactions;
using TMPro;
using UnityEngine;

namespace Code.Interface{
    public class GameSettingsManager : MonoBehaviour{
        public Selections mapSelection;
        public Selections gameModeSelection;
        public Selections overloadSelection;

        private int _lastMap, _lastGameMode, _lastOverload;

        public List<GameSetting> gameSettingsList = new();
        public readonly Dictionary<string, GameSetting> GameSettings = new();

        [Serializable]
        public class GameSetting{
            public string id;
            public GameObject gameObject;
            public TextMeshProUGUI text;
        }


        private void OnValidate(){
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
            
            UpdateScreen();
        }

        private void FixedUpdate(){
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
            Debug.Log(metaData.gameTime);

            GameSettings["Overload"].text.text =
                $"OVERLOAD: {overloadSelection.options[_lastOverload].optionName}";
            GameSettings["Map"].text.text = $"MAP: {mapSelection.options[_lastMap].optionName}";
            GameSettings["GameMode"].text.text =
                $"GAME MODE: {gameModeSelection.options[_lastGameMode].optionName}";
            GameSettings["GameTime"].text.text = $"GAME TIME: {metaData.gameTime}s";
        }

        [Serializable]
        public class OverloadMetaData{
            public int gameTime;
        }
    }
}