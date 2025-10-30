using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Robot
{
    public class ConsumablesUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _consumableIconPrefab;
        [SerializeField] private GameObject _background;
        private List<ConsumableData> _availableConsumables= new List<ConsumableData>();
        private List<GameObject> _consumableIcons = new List<GameObject>();
        
        private void Start()
        {
            LateStartSystem.ExecuteOnLateStart(SetupUI);
        }

        private void SetupUI()
        {
            _availableConsumables = Inventory.Source.GetUniqueItemsOfType<ConsumableData>();
            if (_availableConsumables.Count == 0)
            {
                _background.SetActive(false);
            }
            else
            {
                _background.SetActive(true);
                foreach (var consumable in _availableConsumables)
                {
                    _consumableIcons.Add(Instantiate(_consumableIconPrefab, _background.transform));
                    var ui = _consumableIcons[_consumableIcons.Count-1].GetComponent<ConsumableIconUI>();
                    ui.Setup(consumable.Icon,Inventory.Source.GetItemQuantity(consumable));
                }
            }
        }
    }
}
