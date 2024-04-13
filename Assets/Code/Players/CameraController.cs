using System;
using Code.Interface.Settings;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class CameraController : NetworkBehaviour{
        [Header("References")] 
        public Camera worldCamera;
        public Transform cameraHolder;
        public Transform cameraPosition;

        private void Start(){
            if (isLocalPlayer){
                cameraHolder.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else{
                cameraHolder.gameObject.SetActive(false);
            }
            
            SettingsMenu.Singleton.LoadingSettings.AddListener(LoadSettings);
            LoadSettings();
        }

        private void LateUpdate(){
            if(!isLocalPlayer) return;

            cameraHolder.position = cameraPosition.position;
        }

        private void LoadSettings(){
            if (PlayerPrefs.HasKey("fov")){
                worldCamera.fieldOfView = (int)PlayerPrefs.GetFloat("fov");
            }
            else{
                Application.targetFrameRate = 120;
                PlayerPrefs.SetFloat("fov", worldCamera.fieldOfView);
            }
        }
    }
}