using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewChips", menuName = "Inventory/Item/Chips")]
    public class ChipData : ScriptableObject
    {
        public Sprite ChipSprite;
        public string ChipName;
        public int ChipWidth;
        public int ChipHeight;
        public bool IsRotable;
    }
}
