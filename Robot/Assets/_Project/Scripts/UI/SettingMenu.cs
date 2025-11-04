using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class SettingMenu : MonoBehaviour
    {
        [SerializeField] private Slider _musicVolumeSlider, _sfxVolumeSlider;
        private SettingsSaveFileData _saveFileData;

        private void Start()
        {
            LoadFromSaveSystem();
            _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        private void SetSFXVolume(float value)
        {
            _saveFileData.SoundVolume = value;
            AudioManager.Source.SetSFXVolume(value);
        }

        private void SetMusicVolume(float value)
        {
            _saveFileData.MusicVolume = value;
            AudioManager.Source.SetMusicVolume(value);
        }
        
        private void InitializeSFXVolume(float value)
        {
            _sfxVolumeSlider.value = value;
            AudioManager.Source.SetSFXVolume(value);
        }

        private void InitializeMusicVolume(float value)
        {
            _musicVolumeSlider.value = value;
            AudioManager.Source.SetMusicVolume(value);
        }
        
        public void LoadFromSaveSystem()
        {
            if (!_saveFileData)
            {
                _saveFileData = SaveSystemManager.Source.GetFileData<SettingsSaveFileData>();
                if (!_saveFileData)
                {
                    _saveFileData = ScriptableObject.CreateInstance<SettingsSaveFileData>();
                    _saveFileData.MusicVolume = 1;
                    _saveFileData.SoundVolume = 1;
                    SaveToSaveFile();
                }
            }

            InitializeSFXVolume(_saveFileData.SoundVolume);
            InitializeMusicVolume(_saveFileData.MusicVolume);
        }

        public void SaveToSaveFile()
        {
            SaveSystemManager.Source.SaveFileData(_saveFileData);
        }
    }
}
