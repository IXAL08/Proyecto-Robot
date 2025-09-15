using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class InventoryChipsUI : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _inventoryGridLayoutGroup;

        private GridSlotUI[,] _uiInventoryGridSlots;

        private void Start()
        {
            _uiInventoryGridSlots = new GridSlotUI[InventoryChips.Source.InventoryRows,InventoryChips.Source.InventoryColums];
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            int i = 0;
            foreach (Transform child in _inventoryGridLayoutGroup.transform)
            {
                int x = i % InventoryChips.Source.InventoryColums;
                int y = i / InventoryChips.Source.InventoryColums;

                GridSlotUI slot = child.GetComponent<GridSlotUI>();
                slot.AssignCoord(x, y);

                _uiInventoryGridSlots[x,y] = slot;
                i++;
            }
        }
    }
}
