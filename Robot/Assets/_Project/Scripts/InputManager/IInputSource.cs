using System;
using UnityEngine;

namespace Robot
{
    public interface IInputSource
    {
        event Action BackToOnPlay;
        
        //Pause
        event Action OpenUIPause;
        event Action CloseUIPause;

        //OnPlayMode
        event Action OpenChipsInventory;
        event Action<float> MovePlayer;
        event Action Jump;
        event Action Dash;
        event Action Attack;

        //OnInventoryChips
        event Action CloseChipsInventory;
        event Action DeleteChip;
        event Action RotateChip;
    }
}
