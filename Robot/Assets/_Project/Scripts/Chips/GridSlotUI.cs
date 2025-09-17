using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class GridSlotUI : MonoBehaviour
    {
        [SerializeField] private int _x, _y;
        [SerializeField] private Image _slotImage; //Temporal

        public int X => _x;
        public int Y => _y;

        private void Start()
        {
            InventoryChips.Source.OnItemPlaced += SlotOccupied;
            InventoryChips.Source.OnItemRemoved += FreeSlot;
        }

        private void OnDisable()
        {
            InventoryChips.Source.OnItemPlaced -= SlotOccupied;
            InventoryChips.Source.OnItemRemoved -= FreeSlot;
        }

        public void AssignCoord(int x, int y)
        {
            _x = x;
            _y = y;
        }

        private void SlotOccupied(int row, int column)
        {
            if (row == _x && column == _y)
            {
                _slotImage.color = Color.red;
            }
        }

        private void FreeSlot(int row, int column)
        {
            if (row == _x && column == _y)
            {
                _slotImage.color = Color.white;
            }
        }
    }
}
