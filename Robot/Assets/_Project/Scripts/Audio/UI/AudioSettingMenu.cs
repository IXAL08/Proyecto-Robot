using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class AudioSettingMenu : MonoBehaviour
    {
        [SerializeField] private Slider _bgmSlider, _sfxSlider;
        private void Start()
        {
            LoadVolumeValues();
            _sfxSlider.onValueChanged.AddListener(AudioManager.Source.SetSFXVolume);
            _bgmSlider.onValueChanged.AddListener(AudioManager.Source.SetMusicVolume);
        }

        private void LoadVolumeValues()
        {
            _sfxSlider.value = AudioManager.Source.CurrentSFXVolume;
            _bgmSlider.value = AudioManager.Source.CurrentMusicVolume;
        }

    }
}
