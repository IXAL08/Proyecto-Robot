using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class InventoryChipsUI : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _inventoryGridLayoutGroup;

        private GridSlotUI[,] _uiInventoryGridSlots;
        private Vector2 _gridLayoutGroupCellsize, _gridLayoutGroupSpacing;

        public Vector2 GridLayoutGroupCellsize => _gridLayoutGroupCellsize;
        public Vector2 GridLayoutGroupSpacing => _gridLayoutGroupSpacing;

        private void Start()
        {
            _uiInventoryGridSlots = new GridSlotUI[InventoryChips.Source.InventoryRows,InventoryChips.Source.InventoryColums];
            _gridLayoutGroupCellsize = new Vector2(_inventoryGridLayoutGroup.cellSize.x,_inventoryGridLayoutGroup.cellSize.y);
            _gridLayoutGroupSpacing = new Vector2(_inventoryGridLayoutGroup.spacing.x, _inventoryGridLayoutGroup.spacing.y);
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            int i = 0;
            foreach (Transform child in _inventoryGridLayoutGroup.transform)
            {
                int x = i % InventoryChips.Source.InventoryColums;
                int y = i / InventoryChips.Source.InventoryColums;

                GridSlotUI slot = child.GetComponentInChildren<GridSlotUI>();
                slot.AssignCoord(x, y);

                _uiInventoryGridSlots[x,y] = slot;
                i++;
            }
        }
    }
}
