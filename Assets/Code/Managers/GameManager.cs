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
        private bool _ready;
        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }
        
        [Serializable]
        public class GameSetting{
            public GameMode gameMode = GameMode.None;
            [Space]
            public string countdownPrompt;
            public float gameTime = 90f;
            [Space]
            public bool respawn = true;
            public int lives = -1;
        }

        private void Start(){
            ScreenCover.Singleton.FadeOut();
            CursorManager.Singleton.ResetHide();
        }

        private void FixedUpdate(){
            if(!isServer) return;
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
            gameMode = Manager().localPlayer.gameMode;
            GameSetting currentGameSetting = gameSettings.FirstOrDefault(gameSetting => gameSetting.gameMode == gameMode);
            Countdown.Singleton.StartCountdown(currentGameSetting.gameTime, currentGameSetting.countdownPrompt);
            SetUpGameMode();
            Invoke(nameof(FreezePlayers), currentGameSetting.gameTime);
            Invoke(nameof(PrepairSceneSwitch), currentGameSetting.gameTime + 1f);
        }

        private void PrepairSceneSwitch(){
            if(!isServer) return;
            ScreenCover.Singleton.FadeIn();
            UnTagPlayers();
            Invoke(nameof(SwitchScene), 1.5f);
        }

  

        private void SetUpGameMode(){
            if(!isServer) return;
            switch (gameMode){
                case GameMode.Tag:
                    SetUpTag();
                    break;
            }
        }
        
        private void UnTagPlayers(){
            foreach (GamePlayer gamePlayer in  Manager().Players){
                gamePlayer.GetComponent<Tag>().SetTagged(false);
            }
        }


        private void SetUpTag(){
            if(!isServer) return;
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

        private void SwitchScene(){
            ServerChangeScene();
        }
        
        [Command(requiresAuthority = false)]
        private void ServerChangeScene(){
            Manager().ServerChangeScene("Lobby");
        }
    }
}