using System;
using UnityEngine;

namespace Robot
{
    public class CollectableItem : MonoBehaviour
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _amount;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Inventory.Source.AddItemToInventory(_itemData, _amount);
                Destroy(gameObject);
            }
        }
    }
}
