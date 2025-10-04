using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class ChipUIHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas _chipCanva;
        private CanvasGroup _chipCanvasGroup;
        private RectTransform _chipPivotRectTransform;
        private Chip _chip;
        private bool _isDragging, _firsttouch;


        private void Awake()
        {
            _chipPivotRectTransform = GetComponent<RectTransform>();
            _chip = GetComponentInChildren<Chip>();
            _chipCanvasGroup = GetComponent<CanvasGroup>();
            _chipCanva = GetComponentInParent<Canvas>();
        }
        private void Update()
        {
            if (_isDragging && Input.GetKeyDown(KeyCode.R)) 
            {
                ChipInventoryManager.Source.RotateChip(_chipPivotRectTransform, _chip);
                ChipInventoryUIManager.Source.ApplyVisualRotation(_chipPivotRectTransform, _chip.ChipData.RotationSteps);
            } 
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            if (!_firsttouch)
            {
                ChipInventoryUIManager.Source.ResizeUIOnHandle((RectTransform)_chip.transform, _chip.ChipData, ChipInventoryUIManager.Source.GridLayoutGroupCellsize, ChipInventoryUIManager.Source.GridLayoutGroupSpacing);
                _firsttouch = true;
            }
            ChipInventoryUIManager.Source.ApplyVisualRotation(_chipPivotRectTransform, _chip.ChipData.RotationSteps);
            _chipCanvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _chipPivotRectTransform.anchoredPosition += eventData.delta / _chipCanva.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _chipCanvasGroup.blocksRaycasts = true;
            _isDragging = false;
            
            var slotUI = eventData.pointerEnter?.GetComponent<GridSlotUI>();


            if (slotUI != null) {
                TryPlaceChip(slotUI, _chip);
            }
            else
            {
                print("chip no encontrado");
            }
        }

        private void TryPlaceChip(GridSlotUI slot, Chip chip)
        {
            var newCoords = ChipInventoryManager.Source.GetNewChipCoordinates(slot.Y, slot.X, chip);

            if (!ChipInventoryManager.Source.IsWithinBounds(newCoords) && chip.HasBeenPlaced)
            {
                ChipInventoryUIManager.Source.SnapToLastSlot(chip.CurrentRowPlaced, chip.CurrentColumnPlaced, _chipPivotRectTransform, _chipCanva, chip);
                Debug.Log("❌ El chip no cabe en el grid (fuera de los límites)");
                return;
            }

            if (ChipInventoryManager.Source.CanPlaceChip(slot.Y, slot.X, chip) ||  ChipInventoryManager.Source.IsAlreadyAtPosition(newCoords, chip.CoordinatesOccupiedOnGrid))
            {
                if (!chip.HasBeenPlaced)
                {
                    ChipInventoryManager.Source.PlaceChip(slot.Y, slot.X, chip);
                    ChipInventoryUIManager.Source.SnapToSlot(slot, _chipPivotRectTransform, _chipCanva);
                    Debug.Log("✅ El chip ha sido colocado en el grid");
                }
                else
                {
                    ChipInventoryManager.Source.UnPlaceChip(chip);
                    ChipInventoryManager.Source.PlaceChip(slot.Y, slot.X, chip);
                    ChipInventoryUIManager.Source.SnapToSlot(slot, _chipPivotRectTransform, _chipCanva);
                    Debug.Log("🔄 El chip ha cambiado de lugar");
                }
            }
            else
            {
                ChipInventoryUIManager.Source.SnapToLastSlot(chip.CurrentRowPlaced,chip.CurrentColumnPlaced, _chipPivotRectTransform, _chipCanva, chip);
            }
        }
    }
}
