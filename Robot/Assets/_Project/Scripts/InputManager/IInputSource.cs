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
        event Action OpenInventory;
        event Action OpenCraftingMenu;
        event Action<float> MovePlayer;
        event Action Jump;
        event Action Dash;
        event Action Attack;

        //OnInventoryChips
        event Action CloseChipsInventory;
        event Action DeleteChip;
        event Action RotateChip;

        //OnInventory
        event Action CloseInventory;

        //OnCraftingMenu
        event Action CloseCraftingMenu;
    }
}
