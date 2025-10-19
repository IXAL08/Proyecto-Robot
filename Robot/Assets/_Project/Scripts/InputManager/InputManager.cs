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
        public event Action<float> MovePlayer;
        public event Action Jump;
        public event Action Dash;
        public event Action Attack;

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
                case GameState.OnCraftingMenu:
                    CheckOnCraftingMenuInputs();
                    break;
                case GameState.OnInventoryMenu:
                    CheckOnInventoryMenuInputs();
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
            float horizontalInput = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenUIPause?.Invoke();
                GameStateManager.Source.ChangeState(GameState.OnPause);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenChipsInventory?.Invoke();
                GameStateManager.Source.ChangeState(GameState.OnChipMenu);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                MovePlayer?.Invoke(horizontalInput);
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                Jump?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Dash?.Invoke();
            }

            if (Input.GetButton("Fire1"))
            {
                Attack?.Invoke();
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

        private void CheckOnCraftingMenuInputs()
        {
            /// Coloca los inputs aqui
        }

        private void CheckOnInventoryMenuInputs()
        {
            /// colocar los inputs
        }
    }
}
