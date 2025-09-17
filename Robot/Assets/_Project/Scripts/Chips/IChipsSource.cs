using System;
using UnityEngine;

namespace Robot
{
    public interface IChipsSource
    {
        event Action<int, int> OnChipPlaced;
        event Action<int, int> OnChipRemoved;

        bool CanPlaceChip(int row, int column, ChipData chipData);
        void PlaceChip(int row, int column, ChipData chipData);
        void UnPlaceChip();
        int InventoryColums {get;}
        int InventoryRows {get;}
    }
}
