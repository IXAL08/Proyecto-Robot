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

        private void Awake()
        {
            _chipRectTransform = GetComponent<RectTransform>();
            _chipCanvasGroup = GetComponent<CanvasGroup>();

            _chipCanva = GetComponentInParent<Canvas>();
            _initialChipPosition = _chipRectTransform.anchoredPosition;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
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
            if (InventoryChips.Source.CanPlaceItem(slotUI.X, slotUI.Y, _chipData))
            {
                InventoryChips.Source.PlaceItem(slotUI.X, slotUI.Y, _chipData);
                transform.SetParent(slotUI.transform);
                _chipRectTransform.anchoredPosition = Vector2.zero;
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
        }
    }
}
