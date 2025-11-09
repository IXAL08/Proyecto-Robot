using UnityEngine;

namespace Robot
{
    public class RangeAttackV2 : MonoBehaviour
    {
        [Header("Bullet Setup")]
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private float _bulletDamage;
        [SerializeField] private float _bulletSpeed;

        [Header("Aim Direction")]
        [SerializeField] private ShootDirection _direction;
        [SerializeField] private Transform _firePoint;

        [Header("Attack setup")]
        [SerializeField] private float _fireRate = 1f;

        private float _nextFireTime = 0;
        private bool _isActive = false;

        private void Start()
        {
            InputManager.Source.Attack += OnButtonPressed;
            PlayerStatsManager.Source.OnRangeChipActivation += ActiveRangeAttack;
            PlayerStatsManager.Source.OnBaseStatsChanged += UpdateBulletDamage;
        }

        private void OnDestroy()
        {
            InputManager.Source.Attack -= OnButtonPressed;
            PlayerStatsManager.Source.OnRangeChipActivation -= ActiveRangeAttack;
            PlayerStatsManager.Source.OnBaseStatsChanged -= UpdateBulletDamage;
        }

        private void Shoot()
        {
            var bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            var bulletSetup = bullet.GetComponent<Bullet>();
            var direction = (_direction.AimDirection - _firePoint.position).normalized;

            bulletSetup.SetDirection(direction);
            bulletSetup.SetSpeed(_bulletSpeed);
            bulletSetup.SetDamage(_bulletDamage);
        }

        private void OnButtonPressed()
        {
            if (GameStateManager.Source.CurrentGameState != GameState.OnPlay) return;
            if (!_isActive) return;

            if (Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + _fireRate;
            }
        }

        private void ActiveRangeAttack(bool value)
        {
            _isActive = value;
            _direction.gameObject.SetActive(value);
        }

        private void UpdateBulletDamage(float x, float y, float Damage)
        {
            _bulletDamage = Damage;
        }
    }
}
