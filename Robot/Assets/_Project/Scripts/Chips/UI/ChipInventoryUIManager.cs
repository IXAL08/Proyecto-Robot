using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class ChipInventoryUIManager : Singleton<IInventoryChipUISource>, IInventoryChipUISource
    {
        [SerializeField] private GridLayoutGroup _inventoryGridLayoutGroup;
        [SerializeField] private RectTransform _displayRectTransform;
        [SerializeField] private TextMeshProUGUI _healthStat, _attackStat, _speedStat;
        private GridSlotUI[,] _inventorySlotUI;
        private Vector2 _gridLayoutGroupCellsize, _gridLayoutGroupSpacing;

        public RectTransform DisplayRectTransform => _displayRectTransform;
        public Vector2 GridLayoutGroupCellsize => _gridLayoutGroupCellsize;
        public Vector2 GridLayoutGroupSpacing => _gridLayoutGroupSpacing;
        private void Start()
        {
            InitializeGridData();
            IntializeGridSlot();
            RefreshPlayerStats();
        }

        private void InitializeGridData()
        {
            _inventorySlotUI = new GridSlotUI[ChipInventoryManager.Source.InventoryRows,ChipInventoryManager.Source.InventoryColumns];
            _gridLayoutGroupCellsize = _inventoryGridLayoutGroup.cellSize;
            _gridLayoutGroupSpacing = _inventoryGridLayoutGroup.spacing;
        }

        public void RefreshPlayerStats()
        {
            _healthStat.text = (PlayerStatsManager.Source.PlayerMaxHealth/10).ToString();
            _attackStat.text = (PlayerStatsManager.Source.PlayerDamage/10).ToString();
            _speedStat.text = PlayerStatsManager.Source.PlayerMovementSpeed.ToString();
        }

        private void IntializeGridSlot()
        {
            int i = 0;

            foreach (Transform gridslot in _inventoryGridLayoutGroup.transform)
            {
                int x = i % ChipInventoryManager.Source.InventoryColumns;
                int y = i / ChipInventoryManager.Source.InventoryColumns;
                
                GridSlotUI slot = gridslot.GetComponentInChildren<GridSlotUI>();
                slot.AssignCoord(x, y);

                _inventorySlotUI[y, x] = slot;
                i++;
            }
        }

        public void SnapToSlot(GridSlotUI slot, Transform pivotTransform, Canvas canvas)
        {
            pivotTransform.SetParent(slot.transform);
            pivotTransform.localPosition = Vector2.zero;
            pivotTransform.SetParent(canvas.transform);
        }

        public void SnapToLastSlot(int row, int column, Transform pivotTransform, Canvas canvas, Chip chip)
        {
            chip.RestoreLastStepAndShape();
            ApplyVisualRotation((RectTransform)pivotTransform, chip.CurrentRotationStep);
            pivotTransform.SetParent(_inventorySlotUI[row,column].transform);
            pivotTransform.localPosition = Vector2.zero;
            pivotTransform.SetParent(canvas.transform);
        }

        public void ReturnToDisplay(RectTransform ChipPivotRectTransform, Chip chip)
        {
            ChipPivotRectTransform.SetParent(_displayRectTransform);
            ChipPivotRectTransform.localPosition = Vector2.zero;
            chip.GetComponent<RectTransform>().sizeDelta = chip.SizeDeltaChip;
        }

        public void ResizeUIOnHandle(RectTransform chipRectTransform, ChipData chipData, Vector2 cellSize, Vector2 spacing)
        {

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var offset in chipData.Shape)
            {
                if (offset.x < minX) minX = offset.x;
                if (offset.x > maxX) maxX = offset.x;
                if (offset.y < minY) minY = offset.y;
                if (offset.y > maxY) maxY = offset.y;
            }

            int widthCells = (maxX - minX) + 1;
            int heightCells = (maxY - minY) + 1;

            float width = (widthCells * cellSize.x) + ((widthCells - 1) * spacing.x);
            float height = (heightCells * cellSize.y) + ((heightCells - 1) * spacing.y);

            chipRectTransform.sizeDelta = new Vector2(width, height);

        }

        public void ApplyVisualRotation(RectTransform chipRectTransform, int rotationStep)
        {
            float angle = 0;
            angle = rotationStep switch
            {
                0 => 0,
                1 => 90,
                2 => 180,
                3 => 270,
                _ => (float)0,
            };
            chipRectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }

    }
}
