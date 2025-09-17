using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System;

namespace Robot
{
    public class InventoryChips : Singleton<IChipsSource>, IChipsSource
    {
        [SerializeField] private int _inventoryColums, _inventoryRows;
        
        private bool[,] _inventoryGrid;

        public event Action<int, int> OnItemPlaced;
        public event Action<int, int> OnItemRemoved;

        public int InventoryColums => _inventoryColums;
        public int InventoryRows => _inventoryRows;

        private void Start()
        {
            _inventoryGrid = new bool[_inventoryRows, _inventoryColums]; // Inicializacion del grid, colocar aqui los chips guardados
        }

        public bool CanPlaceItem(int row, int column, ChipData chipData)
        {
            for (int x = 0; x < chipData.ChipWidth; x++) 
            {
                for (int y = 0; y < chipData.ChipHeight; y++) 
                {
                    int checkX = row + x; 
                    int checkY = column + y; 

                    if (checkX >= _inventoryRows || checkY >= _inventoryColums)
                        return false;

                    if (_inventoryGrid[checkX, checkY])
                        return false;
                }
            }

            return true;
        }

        public void PlaceItem(int row, int column, ChipData chipData)
        {
            var Coords = GetComponentInChildren<ChipPlacementData>();
            for (int x = 0; x < chipData.ChipWidth; x++)
            {
                for (int y = 0; y < chipData.ChipHeight; y++)
                {
                    _inventoryGrid[row + x, column + y] = true;
                    Coords.SaveCoordinates(new Vector2Int(row + x, column + y));
                    OnItemPlaced?.Invoke(row + x, column + y);
                }
            }
            PrintInventory();
        }

        public void UnPlaceItem()
        {
            var Coords = GetComponentInChildren<ChipPlacementData>();
            foreach (var coord in Coords.CoordinatesOccupiedOnGrid)
            {
                _inventoryGrid[coord.x, coord.y] = false;
                OnItemRemoved?.Invoke(coord.x, coord.y);
            }
            PrintInventory();
        }

        void PrintInventory()
        {
            string output = "";
            for (int y = 0; y < _inventoryColums; y++)
            {
                for (int x = 0; x < _inventoryRows; x++)
                {
                    output += _inventoryGrid[x, y] ? "X " : ". ";
                }
                output += "\n";
            }
            Debug.Log(output);
        }
    }
}
