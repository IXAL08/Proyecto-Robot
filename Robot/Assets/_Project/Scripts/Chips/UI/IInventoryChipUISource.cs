using UnityEngine;

namespace Robot
{
    public interface IInventoryChipUISource 
    {
        //variables
        public Vector2 GridLayoutGroupCellsize { get; }
        public Vector2 GridLayoutGroupSpacing {  get;}
        public RectTransform DisplayRectTransform { get; }

        //funciones
        void RefreshPlayerStats();
        void SnapToLastSlot(int row, int column, Transform pivotTransform, Canvas canvas, Chip chip);
        void ApplyVisualRotation(RectTransform chipRectTransform, int rotationStep);
        void SnapToSlot(GridSlotUI slot, Transform pivotTransform, Canvas canvas);
        void ResizeUIOnHandle(RectTransform chipRectTransform, ChipData chipData, Vector2 cellSize, Vector2 spacing);
        void ReturnToDisplay(RectTransform ChipPivotRectTransform, Chip chip);
    }
}
