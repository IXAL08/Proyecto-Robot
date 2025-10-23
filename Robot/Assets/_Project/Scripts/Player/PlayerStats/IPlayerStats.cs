using System;
using UnityEngine;

namespace Robot
{
    public interface IPlayerStats
    {
        //Eventos
        event Action<float, float, float> OnStatsChanged;
        event Action<float> OnHealthChanged;
        event Action OnPlayerDeath;
        //Variables
        float PlayerMaxHealth { get; }
        float PlayerAttackPower { get; }
        float PlayerSpeedPower { get; }
        
        

        //Funciones
        void AddModifierToPlayer(BonusStatsChip bonusStats);
        void SubstractModifierToPlayer(BonusStatsChip bonusStats);

        void TakeDamage(float damage);

        void Die();

       void Respawn();
    }
}
