using System;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public interface IInventoryChipSource
    {
        //Variables
        int InventoryColumns { get; }
        int InventoryRows { get; }

        //Funciones
        bool CanPlaceChip(int row, int column, Chip chip);
        void PlaceChip(int row, int column, Chip chip);
        void UnPlaceChip(Chip chip);
        bool IsWithinBounds(List<Vector2Int> coordinates);
        void RotateChip(Transform chipPivot, Chip chip);
        bool IsAlreadyAtPosition(List<Vector2Int> newCoords, List<Vector2Int> coordinatesOccupiedOnGrid);
        List<Vector2Int> GetNewChipCoordinates(int row, int column, Chip chip);

        //Eventos
        event Action<int, int> OnSlotOccupied;
        event Action<int, int> OnSlotFreed;
    }
}
