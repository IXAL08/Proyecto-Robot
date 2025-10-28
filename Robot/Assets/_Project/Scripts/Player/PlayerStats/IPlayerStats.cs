using System;
using UnityEngine;

namespace Robot
{
    public interface IPlayerStats
    {
        //Eventos
        event Action<float, float, float> OnBaseStatsChanged;
        event Action OnDamageRecieved;
        event Action OnHealRecieved;
        event Action OnHealthChanges;
        event Action OnPlayerDeath;
        event Action<bool> OnMeleeChipActivation;
        event Action<bool> OnRangeChipActivation;
        event Action<bool> OnDashChipActivation;
        event Action<bool> OnHealthBarActivation;
        event Action<bool> OnConsumiblesUIActivation;

        //Variables
        float CurrentHealth {  get;}
        float PlayerMaxHealth { get; }
        float PlayerDamage { get; }
        float PlayerMovementSpeed { get; }

        //Funciones
        void AddModifierToPlayer(BonusStatsChip bonusStats);
        void SubstractModifierToPlayer(BonusStatsChip bonusStats);
        void TakeDamage(float damage);
        void Heal(float healAmount);
        void Respawn();
    }
}
