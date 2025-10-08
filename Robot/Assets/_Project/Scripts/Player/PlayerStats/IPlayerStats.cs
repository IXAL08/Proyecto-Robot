using UnityEngine;

namespace Robot
{
    public interface IPlayerStats
    {
        //Variables
        float PlayerMaxHealth { get; }
        float PlayerAttackPower { get; }
        float PlayerSpeedPower { get; }

        //Funciones
        void AddModifierToPlayer(BonusStatsChip bonusStats);
        void SubstractModifierToPlayer(BonusStatsChip bonusStats);
    }
}
