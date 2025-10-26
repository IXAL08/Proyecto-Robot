using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "NewConsumableData", menuName = "Inventory/Item/Consumable")]
    public class ConsumableData : ItemData
    {
        public bool isTemporaryEffect;
        public int tempEffectTime;
        public BonusStatsChip BonusStats;
    }
}
