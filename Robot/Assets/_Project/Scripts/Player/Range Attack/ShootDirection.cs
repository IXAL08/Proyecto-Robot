using UnityEngine;

namespace Robot
{
    public class ShootDirection : MonoBehaviour
    {
        [SerializeField] private Vector3 _aimDirection;
        [SerializeField] private Transform _pivot;
        [SerializeField] private float _radius = 2f, _rotationSpeed = 10f;

        public Vector3 AimDirection => _aimDirection;

        private void Update()
        {
            if (GameStateManager.Source.CurrentGameState != GameState.OnPlay) return;

            RotateDirection();
            GetWorldLocation();
        }

        private Vector3 GetWorldLocation()
        {
            _aimDirection = transform.position;
            return _aimDirection;
        }

        private void RotateDirection()
        {
            Vector3 mousePos = Input.mousePosition;

            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(_pivot.position);
            Vector3 dir = (mousePos - playerScreenPos).normalized;

            Vector3 worldDir = new Vector3(dir.x, dir.y, 0f);
            Vector3 forward = _pivot.right;

            float angle = Vector3.Angle(forward, worldDir);
            if (angle > 90f) return;

            Vector3 targetPos = _pivot.position + worldDir * _radius;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _rotationSpeed);

        }

    }
}
