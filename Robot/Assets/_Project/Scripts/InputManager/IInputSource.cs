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

        //OnInventoryChips
        event Action CloseChipsInventory;
        event Action DeleteChip;
        event Action RotateChip;
    }
}
