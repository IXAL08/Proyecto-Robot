using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] ItemData _itemData;
        [SerializeField] int _quantity;
        
        [Header("UI elements")]
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _quantityText;

        public void Setup(ItemData itemData, int quantity)
        {
            _itemData = itemData;
            _quantity = quantity;

            _icon.sprite = _itemData.Icon;
            _quantityText.text = $"x{_quantity.ToString()}";
        }
    }
}
