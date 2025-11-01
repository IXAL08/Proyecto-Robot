using System;
using UnityEngine;

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
        public event Action OpenInventory;
        public event Action CloseInventory;
        public event Action OpenCraftingMenu;
        public event Action CloseCraftingMenu;

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
                UIManager.Source.ClosePause();
            }
        }

        private void CheckOnPlayInputs()
        {
            float horizontalInput = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenUIPause?.Invoke();
                UIManager.Source.OpenPause();
                GameStateManager.Source.ChangeState(GameState.OnPause);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInventory?.Invoke();
                GameStateManager.Source.ChangeState(GameState.OnInventoryMenu);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                OpenCraftingMenu?.Invoke();
                GameStateManager.Source.ChangeState(GameState.OnCraftingMenu);
            }

            if (Input.GetKeyDown(KeyCode.R))
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCraftingMenu?.Invoke();
                BackToOnPlay?.Invoke();
            }
        }

        private void CheckOnInventoryMenuInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory?.Invoke();
                BackToOnPlay?.Invoke();
            }
        }
    }
}
