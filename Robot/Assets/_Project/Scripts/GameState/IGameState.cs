using System;
using UnityEngine;

namespace Robot
{
    public interface IGameState
    {
        event Action<GameState> OnGameStateChanged;
        GameState CurrentGameState { get; }
        void ChangeState(GameState state);
    }
}
