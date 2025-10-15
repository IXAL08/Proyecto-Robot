using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats/GUIBonusEffect")]
    public class GUIBonusEffect : ScriptableObject
    {
        public bool ActiveHealthBar, ActiveEnemyHealth, ActiveDamageVisualizer, ActiveConsumiblesVisualizer;
    }
}
