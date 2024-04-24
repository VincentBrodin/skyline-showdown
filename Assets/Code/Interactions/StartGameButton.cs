using System;
using Code.Interface;
using Code.Managers;
using Code.MapTools;
using Code.Networking;
using Code.Tools;
using Mirror;

namespace Code.Interactions{
    public class StartGameButton : NetworkBehaviour{
        public Selections map;
        public Selections gameMode;
        public Interactive interactive;

        private CustomNetworkManager _manager;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            interactive.OnInteraction.AddListener(Load);
        }

        private void Load(){
            Enum.TryParse(gameMode.options[gameMode.currentSelected].id, out GameMode gameModeToSet);
            UpdateData(gameModeToSet);
            Invoke(nameof(FadeIn), 5f);
            Invoke(nameof(ChangeScene), 6f);
            Countdown.Singleton.StartCountdown(5f, "STARTING IN:");
        }


        private void UpdateData(GameMode gameModeToSet){
            ServerUpdateData(gameModeToSet);
        }

        [Command(requiresAuthority = false)]
        private void ServerUpdateData(GameMode gameModeToSet){
            ClientUpdateData(gameModeToSet);
        }

        [ClientRpc]
        private void ClientUpdateData(GameMode gameModeToSet){
            Manager().localPlayer.gameMode = gameModeToSet;
            IntroSound.Singleton.PlayLockIn();
        }
        
        private void FadeIn(){
            ServerFadeIn();
        }

        [Command(requiresAuthority = false)]
        private void ServerFadeIn(){
            ClientFadeIn();
        }

        [ClientRpc]
        private void ClientFadeIn(){
            ScreenCover.Singleton.FadeIn();
        }

        private void ChangeScene(){
            ServerChangeScene();
        }

        [Command(requiresAuthority = false)]
        private void ServerChangeScene(){
            Manager().ServerChangeScene(map.options[map.currentSelected].id);
        }
    }
}