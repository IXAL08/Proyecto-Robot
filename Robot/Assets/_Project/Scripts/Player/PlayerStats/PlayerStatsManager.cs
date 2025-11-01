using System;
using UnityEngine;

namespace Robot
{
    public class PlayerStatsManager : Singleton<IPlayerStats>, IPlayerStats
    {
        [SerializeField] private PlayerStats _currentBasePlayerStats;
        [SerializeField] private PlayerStats[] _basePlayerStats;
        [Header("CurrentStats")]
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _currentMaxHealth;
        [SerializeField] private float _currentDamage;
        [SerializeField] private float _currentMovementSpeed;
        [Header("GUIElements")]
        [SerializeField] private GameObject _healthBar, _consumiblesUI;
        [Header("PlayerState")]
        [SerializeField] private bool _isDead;

        public float CurrentHealth => _currentHealth;
        public float PlayerMaxHealth => _currentMaxHealth;
        public float PlayerDamage => _currentDamage;
        public float PlayerMovementSpeed => _currentMovementSpeed;

        public event Action<float, float, float> OnBaseStatsChanged;
        public event Action OnDamageRecieved;
        public event Action OnHealRecieved;
        public event Action OnHealthChanges;
        public event Action OnPlayerDeath;
        public event Action<bool> OnMeleeChipActivation;
        public event Action<bool> OnRangeChipActivation;
        public event Action<bool> OnDashChipActivation;
        public event Action<bool> OnHealthBarActivation;
        public event Action<bool> OnConsumiblesUIActivation;

        private void OnEnable()
        {
            InitializeStatManager();
        }

        private void Start()
        {
            ChipInventoryManager.Source.OnChipPlaced += ActiveChipEffects;
            ChipInventoryManager.Source.OnChipRemoved += DeactiveChipEffects;
        }

        private void OnDisable()
        {
            ChipInventoryManager.Source.OnChipPlaced -= ActiveChipEffects;
            ChipInventoryManager.Source.OnChipRemoved -= DeactiveChipEffects;
        }

        private void InitializeStatManager()
        {
            _currentMaxHealth = _currentBasePlayerStats.MaxHealth; ///
            _currentMovementSpeed = _currentBasePlayerStats.SpeedPower; ///  Colocar datos del savesystem
            _currentDamage = _currentBasePlayerStats.AttackPower;///
            _currentHealth = _currentMaxHealth;
            _isDead = false;

            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);
            OnHealthChanges?.Invoke();
        }

        public void AddModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentMaxHealth += bonusStats.BonusHealth;
            _currentMovementSpeed += bonusStats.BonusSpeed;
            _currentDamage += bonusStats.BonusDamage;
            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);

            //float healthPercentage = _currentHealth / (_currentMaxHealth - bonusStats.BonusHealth);
            //_currentHealth = _currentMaxHealth * healthPercentage;
            OnHealthChanges?.Invoke();
        }

        public void SubstractModifierToPlayer(BonusStatsChip bonusStats)
        {
            _currentMaxHealth -= bonusStats.BonusHealth;
            _currentMovementSpeed -= bonusStats.BonusSpeed;
            _currentDamage -= bonusStats.BonusDamage;
            OnBaseStatsChanged?.Invoke(_currentMaxHealth, _currentMovementSpeed, _currentDamage);

            if (_currentHealth > _currentMaxHealth)
            {
                _currentHealth = _currentMaxHealth;
                OnHealthChanges?.Invoke();
            }
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);

            OnDamageRecieved?.Invoke();
            Debug.Log(_currentHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float healAmount)
        {
            if (_isDead) return;

            _currentHealth += healAmount;
            _currentHealth = Mathf.Min(_currentHealth, _currentMaxHealth);

            OnHealRecieved?.Invoke();
        }

        private void Die()
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
            _currentHealth = _currentMaxHealth;

            OnHealthChanges?.Invoke();
        }

        public void ActiveChipEffects(Chip chip)
        {
            if (chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _healthBar.GetComponent<CanvasGroup>().alpha = 1;
                OnHealthBarActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _consumiblesUI.GetComponent<CanvasGroup>().alpha = 1;
                OnConsumiblesUIActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveMeleeAttack)
            {
                _currentBasePlayerStats = _basePlayerStats[0];
                RefreshBaseStats();
                OnHealthChanges?.Invoke();
                OnMeleeChipActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveRangeAttack)
            {
                _currentBasePlayerStats = _basePlayerStats[1];
                RefreshBaseStats();
                OnHealthChanges?.Invoke();
                OnRangeChipActivation?.Invoke(true);
            }

            if (chip.ChipData.BonusStatsChip.ActiveDash)
            {
                OnDashChipActivation?.Invoke(true);
            }
        }

        public void DeactiveChipEffects(Chip chip)
        {
            if (!chip.ChipData.BonusStatsChip.ActiveHealthBar)
            {
                _healthBar.GetComponent<CanvasGroup>().alpha = 0;
                OnHealthBarActivation?.Invoke(false);
            }

            if (!chip.ChipData.BonusStatsChip.ActiveConsumiblesVisualizer)
            {
                _consumiblesUI.GetComponent<CanvasGroup>().alpha = 0;
                OnConsumiblesUIActivation?.Invoke(false);
            }

            if (!chip.ChipData.BonusStatsChip.ActiveMeleeAttack)
            {
                OnMeleeChipActivation?.Invoke(false);
            }

            if (!chip.ChipData.BonusStatsChip.ActiveRangeAttack)
            {
                OnRangeChipActivation?.Invoke(false);
            }

            if (!chip.ChipData.BonusStatsChip.ActiveDash)
            {
                OnDashChipActivation?.Invoke(false);
            }
        }

        private void RefreshBaseStats()
        {
            _currentDamage = _currentBasePlayerStats.AttackPower;
            _currentMovementSpeed = _currentBasePlayerStats.SpeedPower;
            _currentMaxHealth = _currentBasePlayerStats.MaxHealth;
            if(_currentHealth > _currentMaxHealth)
            {
                _currentHealth = _currentMaxHealth;
            }
        }
    }
}
