using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Robot
{
    public class GameStateManager : Singleton<IGameState>, IGameState
    {
        public GameState CurrentGameState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        public void ChangeState(GameState state)
        {
            if (CurrentGameState == state) return;

            CurrentGameState = state;
            OnGameStateChanged?.Invoke(CurrentGameState);
        }

        private void SetPauseState()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") return;

            switch (CurrentGameState)
            {
                case GameState.OnPlay:
                    ChangeState(GameState.OnPause);
                    break;
                case GameState.OnPause:
                    ChangeState(GameState.OnPlay);
                    break;
                case GameState.OnMenus:
                    ChangeState(GameState.OnMenus);
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
        OnMenus,
        OnGameOver
    }
}
