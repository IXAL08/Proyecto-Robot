using UnityEngine;

namespace Robot
{
    public class InventoryChips : Singleton<IChipsSource>, IChipsSource
    {
        [SerializeField] private int _width, _height;
        
        private bool[,] _inventoryGrid;

        private void Start()
        {
            _inventoryGrid = new bool[_width, _height];
        }

        public bool CanPlaceItem(int startX, int startY, ChipData chipData)
        {
            for (int x = 0; x < chipData.ChipWidth; x++)
            {
                for (int y = 0; y < chipData.ChipHeight; y++)
                {
                    if (startX + x >= _width || startY + y >= _height) return false;
                    if (_inventoryGrid[startX + x, startY + y]) return false;
                }
            }
            return true;
        }

        public void PlaceItem(int startX, int startY, ChipData chipData) 
        {
            for (int x = 0; x < chipData.ChipWidth; x++)
            {
                for (int y = 0; y < chipData.ChipHeight; y++)
                {
                    _inventoryGrid[startX + x, startY + y] = true;
                }
            }
        }

    }
}
