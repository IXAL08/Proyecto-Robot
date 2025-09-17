using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class ChipUIHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private ChipData _chipData;

        private Canvas _chipCanva;
        private CanvasGroup _chipCanvasGroup;
        private RectTransform _chipRectTransform;
        private Transform _chipTransform;
        private Vector2 _initialChipPosition;
        private bool _isDragging;

        private void Awake()
        {
            _chipRectTransform = GetComponent<RectTransform>();
            _chipCanvasGroup = GetComponent<CanvasGroup>();

            _chipCanva = GetComponentInParent<Canvas>();
            _initialChipPosition = _chipRectTransform.anchoredPosition;
        }

        private void Update()
        {
            //Temporal
            if (_chipData.IsRotable && Input.GetKeyDown(KeyCode.R) && _isDragging)
            {
                RotateItem();
                Debug.Log("rotado");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            var ChipUI = GetComponentInParent<InventoryChipsUI>();
            ReSizeUIOnHandle(_chipRectTransform, _chipData, ChipUI.GridLayoutGroupCellsize, ChipUI.GridLayoutGroupSpacing);
            _chipTransform = transform.parent;
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
                TryPlaceItem(slotUI);
            }
            else
            {
                ResetItemPosition();
            }
        }

        private void TryPlaceItem(GridSlotUI slotUI)
        {
            var check = GetComponent<ChipPlacementData>();
            if (InventoryChips.Source.CanPlaceItem(slotUI.X, slotUI.Y, _chipData) && !check.HasBeenPlaced)
            {
                InventoryChips.Source.PlaceItem(slotUI.X, slotUI.Y, _chipData);
                transform.SetParent(slotUI.transform);
                SnapOnInventory();
                transform.SetParent(_chipTransform);
                check.ItemPlaced();
                Debug.Log("Item colocado");
            }
            else
            {
                ResetItemPosition();
                Debug.Log("No se puede colocar aquí");
            }
        }

        private void ResetItemPosition()
        {
            transform.SetParent(_chipTransform);
            _chipRectTransform.anchoredPosition = _initialChipPosition;
            var check = GetComponent<ChipPlacementData>();
            if (check.HasBeenPlaced)
            {
                InventoryChips.Source.UnPlaceItem();
                check.ItemRemoved();
            }
        }

        public void RotateItem()
        {
            int temp = _chipData.ChipWidth;
            _chipData.ChipWidth = _chipData.ChipHeight;
            _chipData.ChipHeight = temp;
            var ChipUI = GetComponentInParent<InventoryChipsUI>();
            ReSizeUIOnHandle(_chipRectTransform, _chipData, ChipUI.GridLayoutGroupCellsize, ChipUI.GridLayoutGroupSpacing);
        }

        private void ReSizeUIOnHandle(RectTransform chipRect, ChipData chipData, Vector2 cellSize, Vector2 spacing)
        {
            float width = (chipData.ChipWidth * cellSize.x) + ((chipData.ChipWidth - 1) * spacing.x);
            float height = (chipData.ChipHeight * cellSize.y) + ((chipData.ChipHeight - 1) * spacing.y);
            chipRect.sizeDelta = new Vector2(width, height);
        }

        private void SnapOnInventory()
        {
            transform.localPosition = Vector2.zero;
            _chipRectTransform.position = new Vector3(transform.position.x,transform.position.y, -1);
        }
    }
}
