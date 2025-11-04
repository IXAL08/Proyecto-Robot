using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class SettingMenu : MonoBehaviour
    {
        [SerializeField] private Slider _musicVolumeSlider, _sfxVolumeSlider;

        private void Start()
        {
            LoadVolumeValues();
            _sfxVolumeSlider.onValueChanged.AddListener(AudioManager.Source.SetSFXVolume);
            _musicVolumeSlider.onValueChanged.AddListener(AudioManager.Source.SetMusicVolume);
        }

        private void LoadVolumeValues()
        {
            _sfxVolumeSlider.value = AudioManager.Source.CurrentSFXVolume;
            _musicVolumeSlider.value = AudioManager.Source.CurrentMusicVolume;
        }
    }
}
