using System;
using UnityEngine;

namespace Robot
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _itemUIPrefab;

        private void OnEnable()
        {
            InventoryItemUI[] items = GetComponentsInChildren<InventoryItemUI>();
            foreach (InventoryItemUI item in items)
            {
                Destroy(item.gameObject);
            }
            SetupInventory();
        }

        private void SetupInventory()
        {
            foreach (var item in Inventory.Source.GetInventory())
            {
                Instantiate(_itemUIPrefab, _content).GetComponent<InventoryItemUI>().Setup(item.Item, item.Quantity);
            }
        }
    }
}
