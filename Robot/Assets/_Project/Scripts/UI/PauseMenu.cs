using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton, _settingButton, _quitButton;

        private void Start()
        {
            _resumeButton.onClick.AddListener(ResumeButton);
            _settingButton.onClick.AddListener(SettingsButton);
            _quitButton.onClick.AddListener(QuitButton);
        }

        private void ResumeButton()
        {
            UIManager.Source.ClosePause();
            GameStateManager.Source.ChangeState(GameState.OnPlay);
        }

        private void SettingsButton()
        {
            UIManager.Source.OpenSetting();
        }

        private void QuitButton()
        {
            ///regresar a menu
        }
    }
}
