using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Robot;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerrorConsole
{
    public class ScreenTransitionManager : Singleton<IScreenTransitionSource>, IScreenTransitionSource
    {
        [Header("COMPONENTS")]
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private GameObject _backgroundPanel;
        [SerializeField] private CanvasGroup _spinner;

        [Header("CONFIGURATIONS")]
        [SerializeField] private float _transitionDuration = 1f;
        [SerializeField] private float _loadingAnimDuration = 0.25f;

        public event Action OnTransitionBegan;

        public void TransitionToScene(string sceneName, TransitionType transitionType)
        {
            OnTransitionBegan?.Invoke();
            ChangeGameState(GameState.InTransition);

            switch (transitionType)
            {
                case TransitionType.Fade:
                    TransitionFade(() => GoToScene(sceneName)).Forget();
                    break;
                case TransitionType.Slide:
                    TransitionSlide(() => GoToScene(sceneName)).Forget();
                    break;
            }
        }

        private void ChangeGameState(GameState state)
        {
            if (GameStateManager.Source != null)
            {
                GameStateManager.Source.ChangeState(state);
            }
        }

        public void Transition(Action action, TransitionType transitionType)
        {
            switch (transitionType)
            {
                case TransitionType.Fade:
                    TransitionFade(action).Forget();
                    break;
                case TransitionType.Slide:
                    TransitionSlide(action).Forget();
                    break;
            }
        }

        private async UniTaskVoid TransitionFade(Action onTransition)
        {
            _mainCanvasGroup.alpha = 0;
            _mainCanvasGroup.gameObject.SetActive(true);
            _backgroundPanel.transform.localPosition = new Vector3(0, 0, 0);
            await _mainCanvasGroup.DOFade(1, _transitionDuration).AsyncWaitForCompletion();
            
            await ShowLoadingIcon();
            
            onTransition?.Invoke();
            
            await _mainCanvasGroup.DOFade(0, _transitionDuration).AsyncWaitForCompletion();
            _mainCanvasGroup.gameObject.SetActive(false);
        }
        
        private async UniTaskVoid TransitionSlide(Action onTransition)
        {
            _mainCanvasGroup.gameObject.SetActive(true);
            _mainCanvasGroup.alpha = 1;
            var mainCanvasRect = ((RectTransform)_mainCanvasGroup.transform).rect;
            var screenWidth = mainCanvasRect.width;
            
            _backgroundPanel.transform.localPosition = new Vector3(screenWidth, 0, 0);
            await _backgroundPanel.transform.DOLocalMoveX(0, _transitionDuration).AsyncWaitForCompletion();
            
            await ShowLoadingIcon();

            onTransition?.Invoke();
            
            await _backgroundPanel.transform.DOLocalMoveX(screenWidth, _transitionDuration).AsyncWaitForCompletion();
            _mainCanvasGroup.gameObject.SetActive(false);
            ChangeGameState(GameState.OnPlay);
        }
        
        private async UniTask ShowLoadingIcon()
        {
            _spinner.gameObject.SetActive(true);
            await _spinner.DOFade(1, 0.3f).From(0).AsyncWaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(_loadingAnimDuration));
            await _spinner.DOFade(0, 0.3f).AsyncWaitForCompletion();
            _spinner.gameObject.SetActive(false);
        }
        
        private void GoToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SaveSystemManager.Source.SaveGame();
        }
    }
    
    public enum TransitionType
    {
        Fade,
        Slide
    }
}
