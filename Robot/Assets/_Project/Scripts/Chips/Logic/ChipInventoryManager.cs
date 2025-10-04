using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Robot
{
    public class ChipInventoryManager : Singleton<IInventoryChipSource>, IInventoryChipSource
    {
        [SerializeField] private int _gridRows, _gridColumns;
        [Header("ChipDisplaySpawn")]
        [SerializeField] private GameObject _currentChipOnDisplay;
        [SerializeField] private GameObject _chipPrefab;
        [SerializeField] private List<ChipData> _availableChips;

        private bool[,] _inventoryGrid;
        private int _listIndex;

        public event Action<int, int> OnSlotOccupied;
        public event Action<int, int> OnSlotFreed;
        public event Action OnChipSpawned;
        public event Action OnListEmpty;
        public event Action OnListNotEmpty;

        public int InventoryRows => _gridRows;
        public int InventoryColumns => _gridColumns;
        public GameObject CurrentChipOnDisplay => _currentChipOnDisplay;

        private void Start()
        {
            _inventoryGrid = new bool[_gridRows, _gridColumns];
            print("Inventario inicializado con: " + _inventoryGrid.Length);
            SpawnNewChip();
            PrintInventory();
        }

        public bool CanPlaceChip(int row, int column, Chip chip)
        {
            foreach (var offset in chip.ChipData.Shape)
            {
                int x = row + offset.x;
                int y = column + offset.y;

                if (x >= _gridRows || y >= _gridColumns || x < 0 || y < 0) return false;
                if (_inventoryGrid[x, y]) return false;
            }
            return true;
        }

        public void PlaceChip(int row, int column, Chip chip)
        {
            chip.SetRowAndColumn(row, column);
            foreach (var offset in chip.ChipData.Shape)
            {
                int x = row + offset.x;
                int y = column + offset.y;

                _inventoryGrid[x, y] = true;
                OnSlotOccupied?.Invoke(y, x);
                chip.SaveCoordinates(new Vector2Int(x,y));
            }
            chip.SetPlaced(true);
            chip.SaveCurrentStepAndShape();
            _availableChips.RemoveAt(_listIndex);
            if (_availableChips.Count > 0)
            {
                OnListNotEmpty?.Invoke();
                IsChipHasBeenPlaced();
            }
            else
            {
                OnListEmpty?.Invoke();
                print("ya no hay chips");
            }
            PrintInventory();
        }

        public void UnPlaceChip(Chip chip)
        {
            foreach (var coord in chip.CoordinatesOccupiedOnGrid)
            {
                _inventoryGrid[coord.x, coord.y] = false;
                OnSlotFreed?.Invoke(coord.y, coord.x);
            }
            chip.CoordinatesOccupiedOnGrid.Clear();
            PrintInventory();
        }

        public bool IsWithinBounds(List<Vector2Int> coordinates)
        {
            foreach (var coord in coordinates)
            {
                if (coord.x < 0 || coord.y < 0 || coord.x >= _gridRows || coord.y >= _gridColumns)
                    return false;
            }
            return true;
        }

        public void RotateChip(Transform chipPivot, Chip chip)
        {
            for (int i = 0; i < chip.ChipData.Shape.Count; i++)
            {
                var offset = chip.ChipData.Shape[i];
                chip.ChipData.Shape[i] = new Vector2Int(-offset.y, offset.x);
            }

            chip.ChipData.RotationSteps = (chip.ChipData.RotationSteps + 1) % 4;

            chipPivot.position = Mouse.current.position.ReadValue();
        }

        public bool IsAlreadyAtPosition(List<Vector2Int> newCoords, List<Vector2Int> coordinatesOccupiedOnGrid)
        {
            return newCoords.Any(newCoord => coordinatesOccupiedOnGrid.Contains(newCoord));
        }

        public List<Vector2Int> GetNewChipCoordinates(int row, int column,Chip chip)
        {
            var coords = new List<Vector2Int>();
            foreach (var cell in chip.ChipData.Shape)
            {
                coords.Add(new Vector2Int(row + cell.x, column + cell.y));
            }
            return coords;
        }

        private void IsChipHasBeenPlaced()
        {
            var chip = _currentChipOnDisplay.GetComponentInChildren<Chip>();
            if (chip.HasBeenPlaced)
            {
                SpawnNewChip();
            }
        }

        private void SpawnNewChip()
        {
            _currentChipOnDisplay = Instantiate(_chipPrefab, ChipInventoryUIManager.Source.DisplayRectTransform);
            _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
            OnChipSpawned?.Invoke();
        }

        public void NextChipData()
        {
            _listIndex = (_listIndex + 1) % _availableChips.Count;
            _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
        }

        public void PreviousChipData()
        {
            _listIndex = (_listIndex - 1 + _availableChips.Count) % _availableChips.Count;
            _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
        }

        private void PrintInventory()
        {
            string output = "";
            for (int y = 0; y < _gridRows; y++)
            {
                for (int x = 0; x < _gridColumns; x++)
                {
                    output += _inventoryGrid[y, x] ? "X " : ". ";
                }
                output += "\n";
            }
            Debug.Log(output);
        }
    }
}
