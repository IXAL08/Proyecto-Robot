using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class SettingMenu : MonoBehaviour
    {
        [SerializeField] private Slider _musicVolumeSlider, _sfxVolumeSlider;
        [SerializeField] private Button _backbutton;

        private void Start()
        {
            LoadVolumeValues();
            _backbutton.onClick.AddListener(BackButton);
            _sfxVolumeSlider.onValueChanged.AddListener(AudioManager.Source.SetSFXVolume);
            _musicVolumeSlider.onValueChanged.AddListener(AudioManager.Source.SetMusicVolume);
        }

        private void LoadVolumeValues()
        {
            _sfxVolumeSlider.value = AudioManager.Source.CurrentSFXVolume;
            _musicVolumeSlider.value = AudioManager.Source.CurrentMusicVolume;
        }

        private void BackButton()
        {
            UIManager.Source.CloseSetting();
        }
    }
}
