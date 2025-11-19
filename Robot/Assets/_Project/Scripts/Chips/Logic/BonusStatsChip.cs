using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats/ChipBonusStats")]
    public class BonusStatsChip : ScriptableObject
    {
        [Header("Stats bonus")]
        public float BonusHealth;
        public float BonusDamage;
        public float BonusSpeed;
        [Header("GUI bonus")]
        public bool ActiveHealthBar;
        public bool ActiveEnemyHealth;
        public bool ActiveDamageVisualizer;
        public bool ActiveConsumiblesVisualizer;
        [Header("Skill bonus")]
        public bool ActiveMeleeAttack;
        public bool ActiveRangeAttack;
        public bool ActiveDash;
        [Header("Healing")]
        public float HealingAmount;
    }
}
