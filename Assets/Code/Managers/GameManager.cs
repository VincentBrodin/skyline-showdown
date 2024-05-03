using System;
using System.Collections.Generic;
using System.Linq;
using Code.Interface;
using Code.Networking;
using Code.Players;
using Code.Players.GameModes;
using Code.Tools;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Managers{
    public class GameManager : NetworkBehaviour{

        public List<GameSetting> gameSettings = new();
        [SyncVar]public GameMode gameMode;
        public int gameTime;
        private bool _ready, _gameStarted;
        private float _counter;
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }
        
        [Serializable]
        public class GameSetting{
            public GameMode gameMode = GameMode.None;
            public string countdownPrompt;
        }
        


        private void Start(){
            ScreenCover.Singleton.FadeOut();
            CursorManager.Singleton.ResetHide();
        }

        private void FixedUpdate(){
            if(!isServer) return;
            if (_gameStarted)
                UpdateGameModes();
            
            if (_ready || NetworkServer.isLoadingScene || NetworkClient.isLoadingScene) return;
            _ready = true;
            Countdown.Singleton.StartCountdown(3, "STARTING IN:");
            FreezePlayers();
            Invoke(nameof(UnFreezePlayers), 3);
            Invoke(nameof(StartGame), 3);
        }

        private void FreezePlayers(){
            foreach (GamePlayer player in Manager().Players){
                player.Freeze();
            }
        }
        
        
        private void UnFreezePlayers(){
            foreach (GamePlayer player in Manager().Players){
                player.UnFreeze();
            }
        }
        
        private void StartGame(){
            if(!isServer) return;
            _gameStarted = true;
            gameMode = Manager().localPlayer.gameMode;
            GameSetting currentGameSetting = gameSettings.FirstOrDefault(gameSetting => gameSetting.gameMode == gameMode);
            Countdown.Singleton.StartCountdown(Manager().localPlayer.metaData.gameTime, currentGameSetting.countdownPrompt);
            SetUpGameMode();
            Invoke(nameof(FreezePlayers), gameTime);
            Invoke(nameof(PrepairSceneSwitch), gameTime + 1f);

            foreach (GamePlayer gamePlayer in Manager().Players){
                gamePlayer.SetGameActive(true);
            }
        }

        private void PrepairSceneSwitch(){
            if(!isServer) return;
            ClientScreenCover();
            UnTagPlayers();
            foreach (GamePlayer player in Manager().Players){
                player.SetGameActive(false);
            }
            Invoke(nameof(SwitchScene), 1.5f);
        }

        [ClientRpc]
        private void ClientScreenCover(){
            ScreenCover.Singleton.FadeIn();
        }

  

        private void SetUpGameMode(){
            if(!isServer) return;
            switch (gameMode){
                case GameMode.Tag:
                    SetUpTag();
                    break;
                case GameMode.HideAndSeek:
                    SetUpHideAndSeek();
                    break;
                case GameMode.KingOfTheHill:
                    SetUpKingOfTheHill();
                    break;
            }
        }
        
        
        private void UpdateGameModes(){
            if(!isServer) return;
            switch (gameMode){
                case GameMode.Tag:
                    GiveTagSurvivalScore();
                    break;
                case GameMode.HideAndSeek:
                    GiveTagSurvivalScore();
                    break;
            }
        }

        
        private void UnTagPlayers(){
            foreach (GamePlayer gamePlayer in  Manager().Players){
                gamePlayer.GetComponent<Tag>().SetTagged(false);
            }
        }

        private void SetUpKingOfTheHill(){
            if(!isServer) return;
            ResetMultiplier();
        }

        
        private void ResetMultiplier(){
            foreach (GamePlayer player in Manager().Players){
                player.GetComponent<KnockBack>().SetMultiplier(1);
            }
        }
        


        private void SetUpTag(){
            if(!isServer) return;
            _counter = 5 + Time.time;
            UnTagPlayers();
            int amountOfPlayers = Manager().Players.Count;
            int numberOfTaggers = amountOfPlayers/2;
            List<GamePlayer> gamePlayers = new();
            Manager().Players.CopyTo(gamePlayers);

            for (int i = 0; i < numberOfTaggers; i++){
                int index = Random.Range(0, gamePlayers.Count);
                gamePlayers[index].GetComponent<Tag>().SetTagged(true);
                gamePlayers.RemoveAt(index);
            }
        }

        private void SetUpHideAndSeek(){
            if(!isServer) return;
            _counter = 5 + Time.time;
            UnTagPlayers();
            
            int amountOfPlayers = Manager().Players.Count;
            int numberOfTaggers = amountOfPlayers/2;
            numberOfTaggers = Mathf.Clamp(numberOfTaggers, 0, 2);
            List<GamePlayer> gamePlayers = new();
            Manager().Players.CopyTo(gamePlayers);

            for (int i = 0; i < numberOfTaggers; i++){
                int index = Random.Range(0, gamePlayers.Count);
                gamePlayers[index].GetComponent<Tag>().SetTagged(true);
                gamePlayers.RemoveAt(index);
            }
        }

        private void GiveTagSurvivalScore(){
            if(_counter > Time.time) return;
            _counter = 5 + Time.time;
            foreach (GamePlayer gamePlayer in  Manager().Players){
                if (!gamePlayer.GetComponent<Tag>().tagged){
                    gamePlayer.GiveScore(5, "SURVIVAL BONUS:");
                }
            }
        }

        private void SwitchScene(){
            ServerChangeScene();
        }
        
        [Command(requiresAuthority = false)]
        private void ServerChangeScene(){
            Manager().ServerChangeScene("Lobby");
        }
    }
}