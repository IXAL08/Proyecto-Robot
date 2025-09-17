using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Robot
{
    public class ChipPlacementData : MonoBehaviour
    {
        [SerializeField] private List<Vector2Int> _coordinatesOccupiedOnGrid = new List<Vector2Int>();
        [SerializeField] private bool _hasBeenPlaced;
        [SerializeField] private Transform _chipDisplay; // temporal

        private Vector2 _defaultSizeDelta, _initialPosition;
        private RectTransform _chipRectTransform;

        public List<Vector2Int> CoordinatesOccupiedOnGrid => _coordinatesOccupiedOnGrid;

        private void Start()
        {
            _initialPosition = _chipDisplay.position;
            _chipRectTransform = GetComponent<RectTransform>();
            _defaultSizeDelta = _chipRectTransform.sizeDelta;
            _chipRectTransform.position = _initialPosition;
        }

        public void SnapToSlotInventory(GridSlotUI slotUI, Canvas canvas)
        {
            transform.SetParent(slotUI.transform);
            transform.localPosition = Vector2.zero;
            transform.SetParent(canvas.transform);
            _initialPosition = _chipRectTransform.position;
            _hasBeenPlaced = true;
        }

        public void ResizeUIOnHandle(RectTransform chipRectTransform, ChipData chipData, Vector2 cellSize, Vector2 spacing)
        {
            float width = (chipData.ChipWidth * cellSize.x) + ((chipData.ChipWidth - 1) * spacing.x);
            float height = (chipData.ChipHeight * cellSize.y) + ((chipData.ChipHeight - 1) * spacing.y);
            chipRectTransform.sizeDelta = new Vector2(width, height);
        }

        public void RotateChip(ChipData chipData)
        {
            int temp = chipData.ChipWidth;
            chipData.ChipWidth = chipData.ChipHeight;
            chipData.ChipHeight = temp;
            var ChipUI = GetComponentInParent<InventoryChipsUI>();
            ResizeUIOnHandle(_chipRectTransform, chipData, ChipUI.GridLayoutGroupCellsize, ChipUI.GridLayoutGroupSpacing);
        }

        public void ResetChipPositionFromDisplay()
        {
            if (_hasBeenPlaced)
            {
                _chipRectTransform.position = _initialPosition;
            }
            else 
            {
                _chipRectTransform.position = _initialPosition;
                _chipRectTransform.sizeDelta = _defaultSizeDelta;
                _hasBeenPlaced = false;
            }
        }

        public void ResetChipFromGrid()
        {
            _initialPosition = _chipDisplay.position;
            _chipRectTransform.position = _initialPosition;
            transform.SetParent(_chipDisplay);
            _chipRectTransform.sizeDelta = _defaultSizeDelta;
            _hasBeenPlaced = false;
            InventoryChips.Source.UnPlaceChip();
            _coordinatesOccupiedOnGrid.Clear();
        }

        public void SaveCoordinates(Vector2Int coords)
        {
            _coordinatesOccupiedOnGrid.Add(coords);
        }

        public void TryPlaceChip(GridSlotUI gridSlotUI, ChipData chipData, Canvas canvas)
        {
            if (InventoryChips.Source.CanPlaceChip(gridSlotUI.X, gridSlotUI.Y, chipData) || IsAlreadyAtPosition(gridSlotUI.X, gridSlotUI.Y))
            {
                if (_hasBeenPlaced)
                {
                    InventoryChips.Source.UnPlaceChip();
                    _coordinatesOccupiedOnGrid.Clear();
                    InventoryChips.Source.PlaceChip(gridSlotUI.X, gridSlotUI.Y, chipData);
                    SnapToSlotInventory(gridSlotUI, canvas);
                    Debug.Log("El chip ha cambiado de lugar");
                }
                else
                {
                    InventoryChips.Source.PlaceChip(gridSlotUI.X, gridSlotUI.Y, chipData);
                    SnapToSlotInventory(gridSlotUI, canvas);
                    Debug.Log("El chip ha sido colocado en el grid");
                }
            }
            else
            {
                ResetChipPositionFromDisplay();
                Debug.Log("El chip ha sido devuelto al inventario");
            }
        }

        public bool IsAlreadyAtPosition(int gridSlotX, int gridSlotY)
        {
            return _coordinatesOccupiedOnGrid.Any(coord => coord.x == gridSlotX && coord.y == gridSlotY);
        }
    }
}
