using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class ChipsUIHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
            if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("Slot"))
            {
                transform.SetParent(eventData.pointerEnter.transform);
                _chipRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                transform.SetParent(_chipTransform);
                _chipRectTransform.anchoredPosition = _initialChipPosition;
            }
            _chipCanvasGroup.blocksRaycasts = true;
        }
    }
}
