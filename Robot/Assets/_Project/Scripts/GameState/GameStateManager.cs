using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Robot
{
    public class GameStateManager : Singleton<IGameState>, IGameState
    {
        public GameState CurrentGameState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        private void Start()
        {
            InputManager.Source.BackToOnPlay += SetGameState;
        }

        private void OnDestroy()
        {
            InputManager.Source.BackToOnPlay -= SetGameState;
        }

        public void ChangeState(GameState state)
        {
            if (CurrentGameState == state) return;

            CurrentGameState = state;
            OnGameStateChanged?.Invoke(CurrentGameState);
        }

        private void SetGameState()
        {

            switch (CurrentGameState)
            {
                case GameState.OnPause:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnChipMenu:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnInventoryMenu:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnCraftingMenu:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnTutorial:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnGameOver:
                    ChangeState(GameState.OnGameOver);
                    break;
            }
        }

    }

    public enum GameState
    {
        OnPlay,
        OnPause,
        OnChipMenu,
        OnInventoryMenu,
        OnCraftingMenu,
        OnGameOver,
        InTransition,
        OnTutorial
    }
}
