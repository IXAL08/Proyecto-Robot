using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class GridSlotUI : MonoBehaviour
    {
        [SerializeField] private int _x, _y;
        [SerializeField] private Image _slotImage;

        public int X => _x;
        public int Y => _y;

        private void OnEnable()
        {
            ChipInventoryManager.Source.OnSlotOccupied += SlotOccupied;
            ChipInventoryManager.Source.OnSlotFreed += FreeSlot;
        }

        private void OnDisable()
        {
            ChipInventoryManager.Source.OnSlotOccupied -= SlotOccupied;
            ChipInventoryManager.Source.OnSlotFreed -= FreeSlot;
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
