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
        public GameObject CurrentChipOnDisplay { get; }

        //Funciones
        bool CanPlaceChip(int row, int column, Chip chip);
        void PlaceChip(int row, int column, Chip chip);
        void UnPlaceChip(Chip chip);
        bool IsWithinBounds(List<Vector2Int> coordinates);
        void RotateChip(Transform chipPivot, Chip chip);
        bool IsAlreadyAtPosition(List<Vector2Int> newCoords, List<Vector2Int> coordinatesOccupiedOnGrid);
        List<Vector2Int> GetNewChipCoordinates(int row, int column, Chip chip);
        void NextChipData();
        void PreviousChipData();
        void ReturnChipToList(RectTransform pivotChip, Chip chip);
        void AddToAvailableChips(ChipData chipData);

        //Eventos
        event Action<int, int> OnSlotOccupied;
        event Action<int, int> OnSlotFreed;
        event Action OnChipSpawned;
        event Action OnListEmpty;
        event Action OnListNotEmpty;
        event Action<Chip> OnChipPlaced;
        event Action<Chip> OnChipRemoved;
    }
}
