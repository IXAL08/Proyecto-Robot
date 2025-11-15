using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Robot
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private float _dashSpeed = 22f;
        [SerializeField] private float _dashDuration = 0.15f;
        [SerializeField] private float _dashCooldown = 0.35f;

        private bool _isDashing = false, _isActive = false;
        private float _dashCooldownTimer = 0f;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            PlayerStatsManager.Source.OnDashChipActivation += ActiveDash;
        }

        private void OnDisable()
        {
            PlayerStatsManager.Source.OnDashChipActivation -= ActiveDash;
        }

        private void Update()
        {
            if (_dashCooldownTimer > 0f)
            {
                _dashCooldownTimer -= Time.deltaTime;
            }
        }

        public void DoDash()
        {
            if (!_isActive) return;
            {
                
            }
            if (!_isDashing && _dashCooldownTimer <= 0)
            {
                DashMovement().Forget();
            }
        }

        public bool GetDashing()
        {
            return _isDashing;
        }

        private async UniTask DashMovement()
        {
            _isDashing = true;
            _dashCooldownTimer = _dashCooldown;

            float startTime = Time.time;
            float dashDirection = transform.eulerAngles.y >= 90f ? -1f : 1f;
            float originalY = _rb.linearVelocity.y;

            bool prevGravity = _rb.useGravity;
            _rb.useGravity = false;

            while (Time.time < startTime + _dashDuration)
            {
                _rb.linearVelocity = new Vector3(dashDirection * _dashSpeed, originalY, 0f);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }

            _rb.useGravity = prevGravity;
            _isDashing = false;
        }

        private void ActiveDash(bool value)
        {
            _isActive = value;
        }
    }
}
