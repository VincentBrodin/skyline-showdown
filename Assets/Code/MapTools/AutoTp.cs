using Code.Players;
using UnityEngine;

namespace Code.MapTools{
    public class AutoTp : MonoBehaviour{
        private void OnCollisionEnter(Collision other){
            if (!other.collider.transform.parent) return;
            if (!other.collider.transform.parent.TryGetComponent(out GamePlayer gamePlayer)) return;
            if (!gamePlayer.isLocalPlayer) return;
            
            gamePlayer.Teleport(SpawnPoints.Singleton.spawnPoints[gamePlayer.playerId].position);
            gamePlayer.GiveScore(-25, "OPPPS");
        }
    }
}