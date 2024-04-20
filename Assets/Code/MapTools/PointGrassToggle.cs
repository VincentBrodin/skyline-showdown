using Code.Interface.Settings;
using MicahW.PointGrass;
using UnityEngine;

namespace Code.MapTools{
    public class PointGrassToggle : MonoBehaviour{
        private bool _renderGrass;
        private PointGrassRenderer _pointGrassRenderer;
        private void Start(){
            SettingsMenu.Singleton.LoadingSettings.AddListener(UpdateSettings);
            _pointGrassRenderer = GetComponent<PointGrassRenderer>();
            UpdateSettings();
        }

        private void UpdateSettings(){
            if (PlayerPrefs.HasKey("render_grass")){
                _renderGrass = PlayerPrefs.GetInt("render_grass") == 1;
            }
            else{
                _renderGrass = true;
                PlayerPrefs.SetInt("render_grass", 1);
            }

            _pointGrassRenderer.enabled = _renderGrass;
        }
    }
}