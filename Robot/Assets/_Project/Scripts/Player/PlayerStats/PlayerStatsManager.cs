using System;
using UnityEngine;

namespace Robot
{
    public class PlayerStatsManager : Singleton<IPlayerStats>, IPlayerStats
    {
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private float _currentPlayerMaxHealth;
        [SerializeField] private float _currentPlayerAttackPower;
        [SerializeField] private float _currentPlayerSpeedPower;

        public event Action<float, float, float> OnStatsChanged;

        public float PlayerMaxHealth => _currentPlayerMaxHealth;
        public float PlayerAttackPower => _currentPlayerAttackPower;
        public float PlayerSpeedPower => _currentPlayerSpeedPower;

        private void OnEnable()
        {
            InitializeStats();
        }
        private void InitializeStats()
        {
            _currentPlayerMaxHealth = _playerStats.MaxHealth;
            _currentPlayerAttackPower = _playerStats.AttackPower;
            _currentPlayerSpeedPower = _playerStats.SpeedPower;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth, _currentPlayerAttackPower, _currentPlayerSpeedPower);
        }

        public void AddModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentPlayerMaxHealth += bonusStats.BonusHealth;
            _currentPlayerAttackPower += bonusStats.BonusDamage;
            _currentPlayerSpeedPower += bonusStats.BonusSpeed;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth,_currentPlayerAttackPower,_currentPlayerSpeedPower);
        }

        public void SubstractModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentPlayerMaxHealth -= bonusStats.BonusHealth;
            _currentPlayerAttackPower -= bonusStats.BonusDamage;
            _currentPlayerSpeedPower -= bonusStats.BonusSpeed;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth, _currentPlayerAttackPower, _currentPlayerSpeedPower);
        }
    }
}
