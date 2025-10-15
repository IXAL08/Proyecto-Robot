using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private float _maxHealth, _currentHealth, _damage;
        [SerializeField] private GameObject[] _healthBar;
        [SerializeField] private Sprite _fullHeartSprite, _halfHeartSprite, _emptyHeartSprite;

        private List<GameObject> _hearthActive = new List<GameObject>();
        private void Start()
        {
            _maxHealth = (PlayerStatsManager.Source.PlayerMaxHealth / 10);
            _currentHealth = _maxHealth;
            PlayerStatsManager.Source.OnStatsChanged += UpdateMaxHealth;
            InitializeHealthBar();
        }

        private void OnDisable()
        {
            PlayerStatsManager.Source.OnStatsChanged -= UpdateMaxHealth;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.O)) 
            {
                RecieveDamageUI();
            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                RecieveHealUI();
            }
        }

        public void InitializeHealthBar()
        {
            var RoundHealth = Mathf.Ceil(_maxHealth);
            
            for (int i = 0; i < RoundHealth; i++)
            {
                _healthBar[i].SetActive(true);
                _hearthActive.Add(_healthBar[i]);
            }

            InitializeHealthBarSprite();
        }

        private void InitializeHealthBarSprite()
        {
            foreach (var item in _healthBar)
            {
                var sprite = item.GetComponent<Image>();
                sprite.sprite = _fullHeartSprite;
            }

            if (_maxHealth % 1 == 0.5f)
            {
                _hearthActive.Last().GetComponent<Image>().sprite = _halfHeartSprite;
            }
        }

        private void RecieveDamageUI()
        {
            _currentHealth -= _damage;
            var RemaingHeart = Mathf.Ceil(_maxHealth - _currentHealth);
            int j = 0, lastHeart = 0;

            for (int i = (_hearthActive.Count - 1); j < RemaingHeart; i--)
            {
                _hearthActive[i].GetComponent<Image>().sprite = _emptyHeartSprite;
                lastHeart = i;
                j++;
            }
            
            if (!(_currentHealth % 1 == 0.5f)) return;
            _hearthActive[lastHeart - 1].GetComponent<Image>().sprite = _halfHeartSprite;
        }

        private void RecieveHealUI()
        {
            _currentHealth += _damage; //cambiar _damage por heal

            for (int i = 0; i < Mathf.Ceil(_currentHealth); i++)
            {
                _hearthActive[i].GetComponent<Image>().sprite = _fullHeartSprite;
            }

            if (!(_currentHealth % 1 == 0.5f)) return;
            _hearthActive[((int)Mathf.Ceil(_currentHealth)) - 1].GetComponent<Image>().sprite = _halfHeartSprite;
        }

        private void UpdateMaxHealth(float maxhealth, float y, float z)
        {
            _maxHealth = (maxhealth / 10);
            InitializeHealthBarSprite();
        }
    }
}
