using System;
using Code.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.Interface.Settings{
    public class SettingsMenu : MonoBehaviour{
        public GameObject settingsMenu;
        public GameObject blocker;
        public Button closeButton;
        public Button clearSettings;
        public AudioMixer audioMixer;
        private bool _watingForInput;

        public static SettingsMenu Singleton{ get; private set; }
        public bool isOpen;

        public readonly UnityEvent LoadingSettings = new();

        private void Awake(){
            if (Singleton != null){
                Destroy(gameObject);
            }
            else{
                Singleton = this;
            }

            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        

        private void Start(){
            closeButton.onClick.AddListener(Hide);
            clearSettings.onClick.AddListener(ClearSettings);
            Hide();
            blocker.SetActive(false);
        }

        public void Show(){
            isOpen = true;
            settingsMenu.SetActive(true);
            CursorManager.Singleton.OpenWindow();
        }

        private void Hide(){
            isOpen = false;
            settingsMenu.SetActive(false);
            CursorManager.Singleton.CloseWindow();
        }

        private void ClearSettings(){
            PlayerPrefs.DeleteAll();
            LoadSettings();
        }

        public void LoadSettings(){
            LoadingSettings.Invoke();
            //FPS
            if (PlayerPrefs.HasKey("max_fps")){
                Application.targetFrameRate = (int)PlayerPrefs.GetFloat("max_fps");
            }
            else{
                Application.targetFrameRate = 120;
                PlayerPrefs.SetFloat("max_fps", 120);
            }

            //Vsync
            if (PlayerPrefs.HasKey("vsync")){
                QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync");
            }
            else{
                QualitySettings.vSyncCount = 0;
                PlayerPrefs.SetFloat("vsync", 0);
            }

            //Fullscreen
            if (PlayerPrefs.HasKey("fullscreen")){
                bool hasFullscreen = PlayerPrefs.GetInt("fullscreen") != 0;

                Screen.fullScreen = hasFullscreen;
            }
            else{
                PlayerPrefs.SetInt("fullscreen", 0);
                Screen.fullScreen = false;
            }

            //Resolution
            if (PlayerPrefs.HasKey("resolution")){
                Resolution resolution = Screen.resolutions[^(PlayerPrefs.GetInt("resolution") + 1)];
                bool hasFullscreen = PlayerPrefs.GetInt("fullscreen") != 0;
                Screen.SetResolution(resolution.width, resolution.height, hasFullscreen);
            }
            else{
                PlayerPrefs.SetInt("resolution", 0);
                Resolution resolution = Screen.resolutions[^1];
                bool hasFullscreen = PlayerPrefs.GetInt("fullscreen") == 0;
                Screen.SetResolution(resolution.width, resolution.height, hasFullscreen);
            }

            //Main Volume
            if (PlayerPrefs.HasKey("main_volume")){
                float value = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("main_volume") / 100, 0.00001f, 1f)) * 20;
                audioMixer.SetFloat("main_volume", value);
            }
            else{
                float value = Mathf.Log10(1) * 20;
                audioMixer.SetFloat("main_volume", value);
                PlayerPrefs.SetFloat("main_volume", 100);
            }

            //Music Volume
            if (PlayerPrefs.HasKey("music_volume")){
                float value = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("music_volume") / 100, 0.00001f, 1f)) * 20;
                audioMixer.SetFloat("music_volume", value);
            }
            else{
                float value = Mathf.Log10(1) * 20;
                audioMixer.SetFloat("music_volume", value);
                PlayerPrefs.SetFloat("music_volume", 100);
            }

            //Sfx_Volume
            if (PlayerPrefs.HasKey("sfx_volume")){
                float value = Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("sfx_volume") / 100, 0.00001f, 1f)) * 20;
                audioMixer.SetFloat("sfx_volume", value);
            }
            else{
                float value = Mathf.Log10(1) * 20;
                audioMixer.SetFloat("sfx_volume", value);
                PlayerPrefs.SetFloat("sfx_volume", 100);
            }
            
            if (!PlayerPrefs.HasKey("forward")){
                PlayerPrefs.SetInt("forward", (int)KeyCode.W);
            }
            
            if (!PlayerPrefs.HasKey("left")){
                PlayerPrefs.SetInt("left", (int)KeyCode.A);
            }
            
            if (!PlayerPrefs.HasKey("right")){
                PlayerPrefs.SetInt("right", (int)KeyCode.D);
            }
            
            if (!PlayerPrefs.HasKey("back")){
                PlayerPrefs.SetInt("back", (int)KeyCode.S);
            }
            
            if (!PlayerPrefs.HasKey("jump")){
                PlayerPrefs.SetInt("jump", (int)KeyCode.Space);
            }
            
            if (!PlayerPrefs.HasKey("interactive")){
                PlayerPrefs.SetInt("interactive", (int)KeyCode.E);
            }
            
            if (!PlayerPrefs.HasKey("render_grass")){
                PlayerPrefs.SetInt("render_grass", 1);
            }
            
            if (!PlayerPrefs.HasKey("head_bob")){
                PlayerPrefs.SetFloat("head_bob", 1);
            }
            
            if (!PlayerPrefs.HasKey("head_pitch")){
                PlayerPrefs.SetFloat("head_pitch", 1);
            }
        }

        private void LateUpdate(){
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen && !_watingForInput){
                Hide();
            }
        }

        public void StartWating(){
            _watingForInput = true;
            blocker.SetActive(true);
        }

        public void StopWating(){
            _watingForInput = false;
            blocker.SetActive(false);
        }
    }
}