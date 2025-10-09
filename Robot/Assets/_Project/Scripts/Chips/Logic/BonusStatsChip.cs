using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats/ChipBonusStats")]
    public class BonusStatsChip : ScriptableObject
    {
        public float BonusHealth;
        public float BonusDamage;
        public float BonusSpeed;
    }
}
