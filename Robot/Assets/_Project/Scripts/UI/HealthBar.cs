using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject[] _healthHearts;
        [SerializeField] private Sprite _fullHeartSprite, _halfHeartSprite, _emptyHeartSprite;

        private List<GameObject> _hearthActive = new List<GameObject>();

        private void Awake()
        {
            PlayerStatsManager.Source.OnHealthChanges += InitializeHearts;
            PlayerStatsManager.Source.OnDamageRecieved += RecieveDamageUI;
            PlayerStatsManager.Source.OnHealRecieved += RecieveHealUI;
            InitializeHearts();
        }

        private void OnDisable()
        {
            PlayerStatsManager.Source.OnHealthChanges -= InitializeHearts;
            PlayerStatsManager.Source.OnDamageRecieved -= RecieveDamageUI;
            PlayerStatsManager.Source.OnHealRecieved -= RecieveHealUI;
        }
        private void InitializeHearts()
        {
            ResetHealthBar();

            var RoundHealth = Mathf.Ceil(PlayerStatsManager.Source.PlayerMaxHealth/10);

            for (int i = 0; i < RoundHealth; i++)
            {
                _healthHearts[i].SetActive(true);
                _hearthActive.Add(_healthHearts[i]);
            }

            InitializeSprites();
        }

        private void InitializeSprites()
        {
            float health = PlayerStatsManager.Source.CurrentHealth / 10;
            int FullHeartCount = Mathf.FloorToInt(health);
            bool isHaveHalfHeart = false;

            for (int i = 0; i < _hearthActive.Count; i++)
            {
                if (i < FullHeartCount)
                {
                    _hearthActive[i].GetComponent<Image>().sprite = _fullHeartSprite;
                }
                else if (health % 1 == 0.5f && !isHaveHalfHeart)
                {
                    _hearthActive[i].GetComponent<Image>().sprite = _halfHeartSprite;
                    isHaveHalfHeart = true;
                }
                else
                {
                    _hearthActive[i].GetComponent<Image>().sprite = _emptyHeartSprite;
                }
            }
        }

        private void RecieveDamageUI()
        {
            var Remainghearts = Mathf.Ceil((PlayerStatsManager.Source.PlayerMaxHealth - PlayerStatsManager.Source.CurrentHealth) / 10);
            int j = 0, lastHeart = 0;

            for (int i = (_hearthActive.Count - 1); j < Remainghearts; i--)
            {
                _hearthActive[i].GetComponent<Image>().sprite = _emptyHeartSprite;
                lastHeart = i;
                j++;
            }

            if (!((PlayerStatsManager.Source.CurrentHealth/10) % 1 == 0.5f)) return;
            _hearthActive[lastHeart].GetComponent<Image>().sprite = _halfHeartSprite;
        }

        private void RecieveHealUI()
        {
            for (int i = 0; i < Mathf.Ceil((PlayerStatsManager.Source.CurrentHealth / 10)); i++)
            {
                _hearthActive[i].GetComponent<Image>().sprite = _fullHeartSprite;
            }

            if (!((PlayerStatsManager.Source.CurrentHealth / 10) % 1 == 0.5f)) return;
            _hearthActive[((int)Mathf.Ceil((PlayerStatsManager.Source.CurrentHealth / 10))) - 1].GetComponent<Image>().sprite = _halfHeartSprite;
        }

        private void ResetHealthBar()
        {
            _hearthActive.Clear();

            for (int i = 0; i < _healthHearts.Count(); i++)
            {
                _healthHearts[i].SetActive(false);
            }
        }
    }
}
