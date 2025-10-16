using System;
using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private int _quantity;

        public ItemData Item => _item;
        public int Quantity => _quantity;

        public void SetQuantity(int quantity)
        {
            _quantity = Mathf.Max(0, quantity);
        }

        public InventorySlot(ItemData item, int quantity)
        {
            _item = item;
            _quantity = quantity;
        }
    }
    
    public class Inventory : Singleton<IInventorySource>, IInventorySource
    {
        [SerializeField] private List<InventorySlot> _itemsList;
        [SerializeField] private GameObject _inventoryUIPrefab;
        private GameObject _inventoryUI;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if(!_inventoryUI) SetupUI();
                _inventoryUI.SetActive(!_inventoryUI.activeSelf);
            }
        }

        private void SetupUI()
        {
            _inventoryUI = Instantiate(_inventoryUIPrefab,transform);
            _inventoryUI.SetActive(false);
        }

        public void AddItemToInventory(ItemData newItem, int quantity = 1)
        {
            if (IsItemInInventory(newItem))
            {
                ModifyItemQuantity(newItem, quantity);
            }
            else
            {
                _itemsList.Add(new InventorySlot(newItem, quantity));
            }
        }

        public void RemoveItemFromInventory(ItemData newItem, int quantity = -1)
        {
            ModifyItemQuantity(newItem, quantity);
        }

        private void ModifyItemQuantity(ItemData newItem, int quantity)
        {
            foreach (var item in _itemsList)
            {
                if (item.Item != newItem) continue;
                item.SetQuantity(quantity);
                return;
            }
        }

        public bool IsItemInInventory(ItemData newItem)
        {
            foreach (var item in _itemsList)
            {
                if(item.Item == newItem) return true;
            }

            return false;
        }

        public bool IsItemQuantityInInventory(ItemData newItem, int quantity)
        {
            foreach (var item in _itemsList)
            {
                if (item.Item == newItem)
                {
                    return item.Quantity >= quantity;
                }
            }

            return false;
        }

        public List<InventorySlot> GetInventory()
        {
            return _itemsList;
        }
    }
}
