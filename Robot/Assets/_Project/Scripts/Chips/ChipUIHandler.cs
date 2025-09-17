using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class ChipUIHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private ChipData _chipData;

        private ChipPlacementData _placementData;
        private Canvas _chipCanva;
        private CanvasGroup _chipCanvasGroup;
        private RectTransform _chipRectTransform;
        private bool _isDragging;

        private void Awake()
        {
            _chipRectTransform = GetComponent<RectTransform>();
            _chipCanvasGroup = GetComponent<CanvasGroup>();
            _placementData = GetComponent<ChipPlacementData>();
            _chipCanva = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            //Temporal
            if (_chipData.IsRotable && Input.GetKeyDown(KeyCode.R) && _isDragging)
            {
                _placementData.RotateChip(_chipData);
                Debug.Log("rotado");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            var ChipUI = GetComponentInParent<InventoryChipsUI>();
            _placementData.ResizeUIOnHandle(_chipRectTransform, _chipData, ChipUI.GridLayoutGroupCellsize, ChipUI.GridLayoutGroupSpacing);
            transform.SetParent(_chipCanva.transform);
            _chipCanvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _chipRectTransform.anchoredPosition += eventData.delta / _chipCanva.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _chipCanvasGroup.blocksRaycasts = true;
            _isDragging = false;

            var slotUI = eventData.pointerEnter?.GetComponent<GridSlotUI>();
            if (slotUI != null)
            {
                _placementData.TryPlaceChip(slotUI, _chipData, _chipCanva);
            }
            else
            {
                _placementData.ResetChipFromGrid();
            }
        }

    }
}
