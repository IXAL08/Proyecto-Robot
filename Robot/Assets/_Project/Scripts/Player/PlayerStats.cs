using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats/PlayerBaseStats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Stats Base")]
        public float MaxHealth = 40f;
        public float AttackPower = 1f;
        public float SpeedPower = 5f;
    }
}
