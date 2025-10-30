using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class ConsumableIconUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _quantityText;

        public void Setup(Sprite imageIcon, int quantity)
        {
            _icon.sprite = imageIcon;
            _quantityText.text = quantity.ToString();
        }

        public void UpdateQuantity(int quantity)
        {
            _quantityText.text = quantity.ToString();
        }
    }
}
