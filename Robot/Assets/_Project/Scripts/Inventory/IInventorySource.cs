using System.Collections.Generic;

namespace Robot
{
    public interface IInventorySource
    {
        public void AddItemToInventory(ItemData newItem, int quantity = 1);
        public void RemoveItemFromInventory(ItemData newItem, int quantity = -1);
        public bool IsItemInInventory(ItemData newItem);
        public bool IsItemQuantityInInventory(ItemData newItem, int quantity);

        public List<InventorySlot> GetInventory();
    }
}
