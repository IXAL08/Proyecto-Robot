using System;
using UnityEngine;

namespace Robot
{
    public interface IAudioSource
    {
        //Eventos
        event Action<float> OnSFXVolumeChange;
        event Action<float> OnMusicVolumeChange;

        //Variables
        float CurrentSFXVolume { get; }
        float CurrentMusicVolume { get; }

        //Funciones
        void SetSFXVolume(float volume);
        void SetMusicVolume(float volume);
        void PlayOneShot(string audioName);
        void PlayLevelMusic(string audioName);

        //Sonidos
    }
}
