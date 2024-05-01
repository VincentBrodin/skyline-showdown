using Code.Networking;
using Code.Players;
using Mirror;
using UnityEngine;

namespace Code.Interactions{
    public class InteractiveBuy : MonoBehaviour{
        public Interactive interactive;
        public BuyScreen buyScreen;
        public MeshRenderer meshRenderer;
        public Material canBuyMat;
        public Material canNotButMat;

        private CustomNetworkManager _manager;

        private bool _canBuy = true;

        private CustomNetworkManager Manager(){
            if (_manager == null)
                _manager = NetworkManager.singleton as CustomNetworkManager;

            return _manager;
        }

        private void Start(){
            interactive.OnInteraction.AddListener(Buy);

            BuyScreen.Option currentOption = buyScreen.options[buyScreen.currentSelected];
            bool hasScore = currentOption.price < Manager().localPlayer.score;
            if (hasScore){
                _canBuy = true;
                meshRenderer.material = canBuyMat;
            }
            else{
                _canBuy = false;
                meshRenderer.material = canNotButMat;
            }
        }

        private void FixedUpdate(){
            BuyScreen.Option currentOption = buyScreen.options[buyScreen.currentSelected];
            bool hasScore = currentOption.price <= Manager().localPlayer.score;
            if (hasScore && !_canBuy){
                _canBuy = true;
                meshRenderer.material = canBuyMat;
            }

            if (!hasScore && _canBuy){
                _canBuy = false;
                meshRenderer.material = canNotButMat;
            }

            string prompt = hasScore
                ? $"Buy {currentOption.id} for {currentOption.price}"
                : $"Not enough score for {currentOption.id}";
            interactive.SetPrompt(prompt);
        }

        private void Buy(){
            BuyScreen.Option currentOption = buyScreen.options[buyScreen.currentSelected];
            bool hasScore = currentOption.price <= Manager().localPlayer.score;

            if (!hasScore) return;

            Manager().localPlayer.GetComponent<Inventory>().GiveItem(buyScreen.currentSelected);
            Manager().localPlayer
                .GiveScore(-currentOption.price, $"Bought {currentOption.id}");
        }

        private void OnValidate(){
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