using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Robot
{
    public class ParedTablas : MonoBehaviour, IAttackable
    {
        [SerializeField] private bool _attackableByMelee = true;
        [SerializeField] private bool _attackableByRange = true;
        [SerializeField] private int _healthPoints = 80;
        
        [SerializeField] private GameObject _destroyEffect;
        [SerializeField] private float _destroyEffectDuration = 1;
        
        [Header("Audio")]
        [SerializeField] private string DestroySFX = "DestroyedWall";
        public void TakeDamage(int damage, bool isMelee)
        {
            if (isMelee && _attackableByMelee || !isMelee && _attackableByRange)
            {
                _healthPoints -= damage;
            }

            if (_healthPoints <= 0)
            {
                Die();
                AudioManager.Source.PlayOneShot(DestroySFX);
            }
        }

        public void Die()
        {
            if (_destroyEffect != null)
            {
                _destroyEffect.SetActive(true);
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                var childEffets = _destroyEffect.transform.GetComponentsInChildren<Transform>().ToList();
                childEffets.RemoveAt(0);
                foreach (Transform t in childEffets)
                {
                    t.DOScale(Vector3.zero, _destroyEffectDuration)
                        .SetEase(Ease.InOutQuad);
                }
            }

            Destroy(gameObject,_destroyEffect != null ? _destroyEffectDuration : 0);
        }
    }
}
