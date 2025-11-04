using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
            Inventory.Source.ConsumableAdded += SetupUI;
            InputManager.Source.Consumable1 += UseConsumable1;
            InputManager.Source.Consumable2 += UseConsumable2;
            InputManager.Source.Consumable3 += UseConsumable3;
            InputManager.Source.Consumable4 += UseConsumable4;
        }

        private void OnDestroy()
        {
            Inventory.Source.ConsumableAdded -= SetupUI;
            InputManager.Source.Consumable1 -= UseConsumable1;
            InputManager.Source.Consumable2 -= UseConsumable2;
            InputManager.Source.Consumable3 -= UseConsumable3;
            InputManager.Source.Consumable4 -= UseConsumable4;
        }

        private void SetupUI()
        {
            CleanScreen();
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

        private void CleanScreen()
        {
            foreach (var item in _background.GetComponentsInChildren<ConsumableIconUI>())
            {
                Destroy(item.gameObject);
            }
        }

        private void UseConsumable1()
        {
            UseConsumable(0);
        }
        
        private void UseConsumable2()
        {
            UseConsumable(1);
        }
        
        private void UseConsumable3()
        {
            UseConsumable(2);
        }
        
        private void UseConsumable4()
        {
            UseConsumable(3);
        }
        
        private void UseConsumable(int consumableIndex)
        {
            if (!_availableConsumables[consumableIndex]) return;
            
            if (!_availableConsumables[consumableIndex].isTemporaryEffect)
            {
                PlayerStatsManager.Source.AddModifierToPlayer(_availableConsumables[consumableIndex].BonusStats);
            }
            else
            {
                StartCountdown(_availableConsumables[consumableIndex].tempEffectTime, _availableConsumables[consumableIndex].BonusStats).Forget();
            }
            Inventory.Source.RemoveItemFromInventory(_availableConsumables[consumableIndex]);
            SetupUI();
        }
        
        private async UniTaskVoid StartCountdown(int seconds, BonusStatsChip bonusStats)
        {
            PlayerStatsManager.Source.AddModifierToPlayer(bonusStats);
            await UniTask.Delay(seconds*1000);
            PlayerStatsManager.Source.SubstractModifierToPlayer(bonusStats);
        }
    }
}
