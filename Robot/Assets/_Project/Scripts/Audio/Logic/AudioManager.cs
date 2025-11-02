using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Robot
{
    public class AudioManager : Singleton<IAudioSource>, IAudioSource
    {
        [SerializeField] private AudioDataBase _audioDataBase;
        [SerializeField] private AudioMixer _sfxMixer;
        [SerializeField] private AudioMixer _bgmMixer;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioSource _bgmAudioSource;

        public float CurrentSFXVolume { get; private set; }
        public float CurrentMusicVolume { get; private set; }

        public event Action<float> OnSFXVolumeChange;
        public event Action<float> OnMusicVolumeChange;

        private void OnEnable()
        {
            LoadAudioSettings();
        }

        private void LoadAudioSettings()
        {
            CurrentSFXVolume = 1f;
            CurrentMusicVolume = 1f;

            float sfxDb = Mathf.Log10(CurrentSFXVolume == 0f ? 0.0001f : CurrentSFXVolume) * 20f;
            float musicDb = Mathf.Log10(CurrentMusicVolume == 0f ? 0.0001f : CurrentMusicVolume) * 20f;

            _sfxMixer.SetFloat("sfx_vol", sfxDb);
            _bgmMixer.SetFloat("bgm_vol", musicDb);
        }

        public void PlayLevelMusic(string audioName)
        {
            _bgmAudioSource.clip = _audioDataBase.GetAudio(audioName);
            _bgmAudioSource.Play();
        }

        public void PlayOneShot(string audioName)
        {
            _sfxAudioSource.PlayOneShot(_audioDataBase.GetAudio(audioName));
        }

        public void SetMusicVolume(float volume)
        {
            var volumeMixerValue = Mathf.Clamp01(volume);
            var dB = Mathf.Log10(volumeMixerValue == 0f ? 0.0001f : volumeMixerValue) * 20f;
            _bgmMixer.SetFloat("bgm_vol", dB);

            CurrentMusicVolume = volume;
            OnMusicVolumeChange?.Invoke(volume);
        }

        public void SetSFXVolume(float volume)
        {
            var volumeMixerValue = Mathf.Clamp01(volume);
            var dB = MathF.Log10(volumeMixerValue == 0f ? 0.0001f : volumeMixerValue) * 20f;
            _sfxMixer.SetFloat("sfx_vol", dB);

            CurrentSFXVolume = volume;
            OnSFXVolumeChange?.Invoke(volume);
        }
    }
}
