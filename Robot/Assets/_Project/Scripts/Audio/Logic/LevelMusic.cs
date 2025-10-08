using UnityEngine;

namespace Robot
{
    public class LevelMusic : MonoBehaviour
    {
        [SerializeField] private string _musicName;

        private void Start()
        {
            AudioManager.Source.PlayLevelMusic(_musicName);
        }
    }
}
