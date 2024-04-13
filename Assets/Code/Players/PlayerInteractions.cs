using System;
using Code.Interactions;
using Code.Interface;
using Code.Interface.Settings;
using Mirror;
using UnityEngine;

namespace Code.Players{
    public class PlayerInteractions : NetworkBehaviour{
        public float range = 2;
        public LayerMask interactionLayers = 0;
        public Transform worldCamera;
        public KeyCode interactionKey = KeyCode.E;

        private RaycastHit _rayHitInfo;
        private Interactive _lastInteractive;

        private void Start(){
            SettingsMenu.Singleton.LoadingSettings.AddListener(LoadSettings);
            LoadSettings();
        }

        private void LoadSettings(){
            if (PlayerPrefs.HasKey("interactive")){
                interactionKey = (KeyCode)Enum.ToObject(typeof(KeyCode), PlayerPrefs.GetInt("interactive"));
            }
            else{
                PlayerPrefs.SetInt("interactive", (int)interactionKey);
            }
        }

        private void FixedUpdate(){
            if(!isLocalPlayer) return;
            
            bool rayHit = Physics.Raycast(worldCamera.position, worldCamera.forward, out _rayHitInfo, range, interactionLayers);

            if (!rayHit || !_rayHitInfo.collider.TryGetComponent(out Interactive interactive)){
                if (_lastInteractive)
                    _lastInteractive.StopLooking();
                InteractionPrompt.Singleton.Hide();
                _lastInteractive = null;
                return;
            }

            if (!interactive.active){
                if (_lastInteractive)
                    _lastInteractive.StopLooking();
                InteractionPrompt.Singleton.Hide();
                _lastInteractive = null;
                return;
            }

            if (_lastInteractive == interactive) return;
            
            if (_lastInteractive)
                _lastInteractive.StopLooking();
            _lastInteractive = interactive;
            _lastInteractive.LookAt();
            InteractionPrompt.Singleton.Show(interactionKey, _lastInteractive.Prompt);
        }

        private void Update(){
            if(!isLocalPlayer) return;

            if(!_lastInteractive || !Input.GetKeyDown(interactionKey)) return;
            
            _lastInteractive.Interact();
        }
    }
}