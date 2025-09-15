using UnityEngine;

namespace Robot
{
    public interface IChipsSource
    {
        bool CanPlaceItem(int row, int column, ChipData chipData);
        void PlaceItem(int row, int column, ChipData chipData);
        int InventoryColums {get;}
        int InventoryRows {get;}
    }
}
