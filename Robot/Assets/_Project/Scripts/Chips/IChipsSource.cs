using System;
using UnityEngine;

namespace Robot
{
    public interface IChipsSource
    {
        event Action<int, int> OnItemPlaced;
        event Action<int, int> OnItemRemoved;

        bool CanPlaceItem(int row, int column, ChipData chipData);
        void PlaceItem(int row, int column, ChipData chipData);
        void UnPlaceItem();
        int InventoryColums {get;}
        int InventoryRows {get;}
    }
}
