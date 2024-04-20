using UnityEngine;

namespace Code.Tools{
    public static class GameObjectExtension{
        public static void SetLayer(this GameObject gameObject, int layer, bool setToChildren){
            gameObject.layer = layer;
            if (!setToChildren) return;
            for (int i = 0; i < gameObject.transform.childCount; i++){
                gameObject.transform.GetChild(i).gameObject.SetLayer(layer, true);
            }
        }
    }
}