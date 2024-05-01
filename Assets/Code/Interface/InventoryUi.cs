using System.Collections.Generic;
using UnityEngine;

namespace Code.Interface{
    public class InventoryUi : MonoBehaviour{
        public List<InventoryUiItem> inventoryItems = new();

        public static InventoryUi Singleton{ get; private set; }

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }
        }

        public void Select(int index){
            foreach (InventoryUiItem inventoryUiItem in inventoryItems){
                inventoryUiItem.outline.SetActive(false);
            }

            inventoryItems[index].outline.SetActive(true);
        }

        public void SetCooldown(int index, float percent){
            inventoryItems[index].slider.value = percent;
        }

        public void SetUnlocked(int index, bool unlocked){
            inventoryItems[index].locked.SetActive(!unlocked);
        }

        public void SetUses(int index, int uses){
            inventoryItems[index].uses.text = uses == 0 ? "" : $"{uses}";
        }
    }
}