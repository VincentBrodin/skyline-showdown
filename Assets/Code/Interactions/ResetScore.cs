using System;
using Code.Networking;
using Code.Players;
using Mirror;
using UnityEngine;

namespace Code.Interactions{
    public class ResetScore : NetworkBehaviour{
        [SyncVar] public int interactions;
        public bool hasInteracted;
        public Interactive interactive;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            interactive.OnInteraction.AddListener(OnInteraction);
        }

        private void OnInteraction(){
            if (hasInteracted){
                hasInteracted = false;
                ServerAddInteraction(-1);
            }
            else{
                hasInteracted = true;
                ServerAddInteraction(1);
            }
        }

        [Command(requiresAuthority = false)]
        private void ServerAddInteraction(int amount){
            interactions += amount;

            if (interactions != Manager().Players.Count) return;
            foreach (GamePlayer gamePlayer in Manager().Players){
                gamePlayer.SetScore(0);
            }

            interactions = 0;
            ClientResetButton();
        }

        [ClientRpc]
        private void ClientResetButton(){
            hasInteracted = false;
        }

        private void FixedUpdate(){
            interactive.SetPrompt($"{interactions}/{Manager().Players.Count} want to reset the score");
        }

        protected void OnValidate(){
            if (interactive != null) return;
            Interactive onGameObject = GetComponent<Interactive>();
            Interactive onChild = GetComponentInChildren<Interactive>();

            if (onGameObject){
                interactive = onGameObject;
            }
            else if (onChild){
                interactive = onChild;
            }
        }
    }
}