using System;
using UnityEngine;

namespace Robot
{
    public class PlayerStatsManager : Singleton<IPlayerStats>, IPlayerStats
    {
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private float _currentPlayerMaxHealth, _currentPlayerAttackPower, _currentPlayerSpeedPower;
        [SerializeField] private GameObject _playerHealthBar, _playerConsumibles;
        [SerializeField] private float _currentHealth;
        [SerializeField] private bool _isDead;

        public event Action<float, float, float> OnStatsChanged;
        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDeath;

        public float PlayerMaxHealth => _currentPlayerMaxHealth;
        public float PlayerAttackPower => _currentPlayerAttackPower;
        public float PlayerSpeedPower => _currentPlayerSpeedPower;
        public float CurrentHealth => _currentHealth;
        public bool IsDead => _isDead;
        

        private void OnEnable()
        {
            InitializeStats();
            InitializeHealthSystem();
        }

        private void Start()
        {
            ChipInventoryManager.Source.OnChipPlaced += ActiveGUI;
            ChipInventoryManager.Source.OnChipRemoved += DeactiveGUI;
        }

        private void Start()
        {
            ChipInventoryManager.Source.OnChipPlaced += ActiveGUI;
            ChipInventoryManager.Source.OnChipRemoved += DeactiveGUI;
        }
        private void InitializeStats()
        {
            _currentPlayerMaxHealth = _playerStats.MaxHealth;
            _currentPlayerAttackPower = _playerStats.AttackPower;
            _currentPlayerSpeedPower = _playerStats.SpeedPower;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth, _currentPlayerAttackPower, _currentPlayerSpeedPower);
        }

        private void InitializeHealthSystem()
        {
            _currentHealth = _currentPlayerMaxHealth;
            _isDead = false;
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void AddModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentPlayerMaxHealth += bonusStats.BonusHealth;
            _currentPlayerAttackPower += bonusStats.BonusDamage;
            _currentPlayerSpeedPower += bonusStats.BonusSpeed;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth,_currentPlayerAttackPower,_currentPlayerSpeedPower);
            
            float healthPercentage = _currentHealth / (_currentPlayerMaxHealth - bonusStats.BonusHealth);
            _currentHealth = _currentPlayerMaxHealth * healthPercentage;
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void SubstractModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentPlayerMaxHealth -= bonusStats.BonusHealth;
            _currentPlayerAttackPower -= bonusStats.BonusDamage;
            _currentPlayerSpeedPower -= bonusStats.BonusSpeed;
            OnStatsChanged?.Invoke(_currentPlayerMaxHealth, _currentPlayerAttackPower, _currentPlayerSpeedPower);
            
            if (_currentHealth > _currentPlayerMaxHealth)
            {
                _currentHealth = _currentPlayerMaxHealth;
                OnHealthChanged?.Invoke(_currentHealth);
            }
        }

        public void TakeDamage(float damage)
        {
            if(_isDead) return;
            
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            
            OnHealthChanged?.Invoke(_currentHealth);
            Debug.Log(_currentHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            _isDead = true;
            
            OnPlayerDeath?.Invoke();
            
            if (CheckpointSystem.Instance != null)
            {
                CheckpointSystem.Instance.OnPlayerDeath();
            }
        }

        public void Respawn()
        {
            _isDead = false;
            _currentHealth = _currentPlayerMaxHealth;
        
            OnHealthChanged?.Invoke(_currentHealth);
        }
        
        public void Heal(float healAmount)
        {
            if (_isDead) return;

            _currentHealth += healAmount;
            _currentHealth = Mathf.Min(_currentHealth, _currentPlayerMaxHealth);
        
            OnHealthChanged?.Invoke(_currentHealth);
        }
        public void HealFull()
        {
            if (_isDead) return;

            _currentHealth = _currentPlayerMaxHealth;
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void ActiveGUI(Chip chip)
        {
            if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _playerHealthBar.SetActive(true);
                _playerHealthBar.GetComponent<HealthBar>().InitializeHealthBar();
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _playerConsumibles.SetActive(true);
            }
        }

        public void DeactiveGUI(Chip chip)
        {
            if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _playerHealthBar.SetActive(false);                
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _playerConsumibles.SetActive(false);
            }
        }

        public void ActiveGUI(Chip chip)
        {
            if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _playerHealthBar.SetActive(true);
                _playerHealthBar.GetComponent<HealthBar>().InitializeHealthBar();
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _playerConsumibles.SetActive(true);
            }
        }

        public void DeactiveGUI(Chip chip)
        {
            if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _playerHealthBar.SetActive(false);                
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _playerConsumibles.SetActive(false);
            }
        }
    }
}
