using UnityEngine;

namespace Code.Managers{
    public class CursorManager : MonoBehaviour{

        public static CursorManager Singleton;

        public bool WindowsOpend => _windowsOpen != 0;
        private int _windowsOpen;


        private void Awake(){
            if(Singleton != null) {
                Destroy(gameObject);
            }
            else {
                Singleton = this;
            }

            DontDestroyOnLoad(this);
        }

        public void OpenWindow(){
            _windowsOpen++;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void CloseWindow(){
            _windowsOpen--;

            if (_windowsOpen != 0) return;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ResetHide(){
            _windowsOpen = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ResetShow(){
            _windowsOpen = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}