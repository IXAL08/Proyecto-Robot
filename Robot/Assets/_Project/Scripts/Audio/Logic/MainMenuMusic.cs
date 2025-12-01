using UnityEngine;

namespace Robot
{
    public class MainMenuMusic : MonoBehaviour
    {
        [SerializeField] private string _menuMusicName = "MainMenuMusic";

        private void Start()
        {
            AudioManager.Source.PlayLevelMusic(_menuMusicName);
        }
    }
}
