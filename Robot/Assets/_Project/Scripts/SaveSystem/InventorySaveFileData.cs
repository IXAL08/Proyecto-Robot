using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewInventorySaveFile", menuName = "Save Files/Inventory")]
    public class InventorySaveFileData : FileData
    {
        public List<InventorySlot> _currentItemsList;

        public override void ResetData()
        {
            _currentItemsList.Clear();
        }
    }
}