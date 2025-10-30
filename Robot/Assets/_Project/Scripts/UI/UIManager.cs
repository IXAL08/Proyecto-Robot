using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Robot
{
    public class UIManager : Singleton<IUISource>, IUISource
    {
        [SerializeField] private GameObject _chipUIInventory, _pauseMenu, _settingsMenu;
        [SerializeField] private CanvasGroup _guiUI, _backgroundUI;

        private void Start()
        {
            InputManager.Source.OpenChipsInventory += OpenChipInventory;
            InputManager.Source.CloseChipsInventory += CloseChipInventory;
        }

        private void OnDisable()
        {
            InputManager.Source.OpenChipsInventory -= OpenChipInventory;
            InputManager.Source.CloseChipsInventory -= CloseChipInventory;
        }

        public void OpenPause()
        {
            ShowPauseMenu().Forget();
        }

        public void ClosePause()
        {
            if (_settingsMenu.activeSelf)
            {
                CloseSetting();
                GameStateManager.Source.ChangeState(GameState.OnPause);
            }
            else
            {
                HidePauseMenu().Forget();
            }

        }

        public void OpenSetting()
        {
            ShowSettingsMenu().Forget();
        }

        public void CloseSetting()
        {
            HideSettingsMenu().Forget();
        }

        private void CloseChipInventory()
        {
            _chipUIInventory.SetActive(false);
            ShowGUI();
        }

        private void OpenChipInventory()
        {
            _chipUIInventory.SetActive(true);
            _chipUIInventory.GetComponentInChildren<ChipDisplay>().RefreshChipDescription();
            HideGUI();
        }

        private void HideGUI()
        {
            _guiUI.alpha = 0;
        }

        private void ShowGUI()
        {
            _guiUI.alpha = 1;
        }

        private async UniTask ShowSettingsMenu()
        {
            await _pauseMenu.transform.DOLocalMoveX(1415,0.5f).AsyncWaitForCompletion();
            _pauseMenu.gameObject.SetActive(false);
            _settingsMenu.gameObject.SetActive(true);
            await _settingsMenu.transform.DOLocalMoveY(0, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask HideSettingsMenu()
        {
            await _settingsMenu.transform.DOLocalMoveY(1025, 0.5f).AsyncWaitForCompletion();
            _settingsMenu.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(true);
            await _pauseMenu.transform.DOLocalMoveX(0, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask ShowPauseMenu()
        {
            HideGUI();
            _backgroundUI.gameObject.SetActive(true);
            await _backgroundUI.DOFade(1, 0.5f).AsyncWaitForCompletion();
        }

        private async UniTask HidePauseMenu()
        {
            await _backgroundUI.DOFade(0, 0.5f).AsyncWaitForCompletion();
            ShowGUI();
            _backgroundUI.gameObject.SetActive(false);
        }
    }
}
