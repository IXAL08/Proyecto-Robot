using System.Collections.Generic;

namespace Robot
{
    public interface IInventorySource
    {
        public void LoadFromSaveSystem();
        public void SaveToSaveFile();
        public void AddItemToInventory(ItemData newItem, int quantity = 1);
        public void RemoveItemFromInventory(ItemData newItem, int quantity = -1);
        public bool IsItemInInventory(ItemData newItem);
        public bool IsItemQuantityInInventory(ItemData newItem, int quantity);
        
        public int GetItemQuantity(ItemData item);
        
        public List<T> GetItemsOfType<T>() where T : ItemData;

        public List<InventorySlot> GetInventory();
    }
}
