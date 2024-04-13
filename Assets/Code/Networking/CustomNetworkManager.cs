using System.Collections.Generic;
using Code.Interface;
using Code.Players;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Networking{
    public class CustomNetworkManager : NetworkManager{
        [SerializeField] private GamePlayer gamePlayerPrefab;

        private int _playerIdCounter;

        public readonly UnityEvent OnPlayerAdded = new();
        public readonly UnityEvent OnPlayerRemoved = new();
        public GamePlayer localPlayer;

        public List<GamePlayer> Players{ get; } = new();
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn){
            GamePlayer gamePlayer = Instantiate(gamePlayerPrefab);

            gamePlayer.connectionId = conn.connectionId;
            gamePlayer.playerId = _playerIdCounter;
            _playerIdCounter++;


            NetworkServer.AddPlayerForConnection(conn, gamePlayer.gameObject);
        }

        public void AddPlayer(GamePlayer gamePlayer){
            if(!Players.Contains(gamePlayer))
                Players.Add(gamePlayer);
            gamePlayer.name = $"Player {gamePlayer.playerId}";
            OnPlayerAdded.Invoke();
            
            PlayerList.Singleton.AddPlayer(gamePlayer);
        }

        public void RemovePlayer(GamePlayer gamePlayer){
            Players.Remove(gamePlayer);
            OnPlayerRemoved.Invoke();
            
            PlayerList.Singleton.RemovePlayer(gamePlayer);
        }
    }
}