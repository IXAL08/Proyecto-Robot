using UnityEngine;

namespace  Robot
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item/BasicItem")]
    public class ItemData : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public string Description;
    }
   
}