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

        public readonly UnityEvent<GamePlayer> OnPlayerAdded = new();
        public readonly UnityEvent<GamePlayer> OnPlayerRemoved = new();
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
            OnPlayerAdded.Invoke(gamePlayer);
            
        }

        public void RemovePlayer(GamePlayer gamePlayer){
            Players.Remove(gamePlayer);
            OnPlayerRemoved.Invoke(gamePlayer);
        }
    }
}