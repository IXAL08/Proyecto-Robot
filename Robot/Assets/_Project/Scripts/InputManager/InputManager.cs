using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Robot
{
    public class InputManager : Singleton<IInputSource>, IInputSource
    {
        public GameState _currentGameState;

        public event Action BackToOnPlay;
        public event Action OpenUIPause;
        public event Action CloseUIPause;
        public event Action OpenChipsInventory;
        public event Action CloseChipsInventory;
        public event Action DeleteChip;
        public event Action RotateChip;

        private void Start()
        {
            GameStateManager.Source.OnGameStateChanged += OnGameStatedChanged;
        }

        private void OnDestroy()
        {
            GameStateManager.Source.OnGameStateChanged -= OnGameStatedChanged;
        }

        private void Update()
        {
            ActivateInput();
        }

        private void OnGameStatedChanged(GameState state)
        {
            _currentGameState = state;
        }

        private void ActivateInput()
        {
            switch (_currentGameState)
            {
                case GameState.OnPause:
                    CheckOnPauseInputs();
                    break;
                case GameState.OnPlay:
                    CheckOnPlayInputs();
                    break;
                case GameState.OnChipMenu:
                    CheckOnChipMenuInputs();
                    break;

            }
        }

        private void CheckOnPauseInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUIPause?.Invoke();
                BackToOnPlay?.Invoke();
            }
        }

        private void CheckOnPlayInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenUIPause?.Invoke();
                BackToOnPlay?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenChipsInventory?.Invoke();
                GameStateManager.Source.ChangeState(GameState.OnChipMenu);
            }
        }

        private void CheckOnChipMenuInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseChipsInventory?.Invoke();
                BackToOnPlay?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                DeleteChip?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateChip?.Invoke();
            }
        }
    }
}
