using Code.Players;
using Code.Players.GameModes;
using UnityEngine;

namespace Code.MapTools{
    public class AutoTp : MonoBehaviour{
        public bool losePointsOnTp;
        public int amount = -25;
        public string prompt = "OPPS";

        private void OnCollisionEnter(Collision other){
            if (!other.collider.transform.parent) return;
            if (!other.collider.transform.parent.TryGetComponent(out GamePlayer gamePlayer)) return;
            if (!gamePlayer.isLocalPlayer) return;

            gamePlayer.Teleport(SpawnPoints.Singleton.spawnPoints[gamePlayer.playerId].position);
            if (losePointsOnTp)
                gamePlayer.GiveScore(amount, prompt);
            gamePlayer.GetComponent<KnockBack>().SetMultiplier(1);
        }
    }
}