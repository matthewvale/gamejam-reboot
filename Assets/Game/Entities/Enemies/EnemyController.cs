/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class EnemyController : MonoBehaviour, IDamageHandler
    {
        #region Public Properties



        #endregion

        #region Private Properties

        [SerializeField] private bool _isBoss = false;
        [SerializeField] private float _maxHealth = 100f;
        private Health _health;

        [SerializeField] private Canvas _healthCanvas;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private RectTransform _damageIndicatorParent;
        [SerializeField] private GameObject _damageIndicatorPrefab;
        [SerializeField] private int _maxDamageIndicatorPool = 6;
        private Dictionary<GameObject, DamageIndicator> _damageIndicators = new();

        [SerializeField] private GameObject[] _objectsToActivate;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _health = new Health();
            _health.SetHealth(_maxHealth);

            if (_healthSlider != null)
            {
                _healthSlider.minValue = 0f;
                _healthSlider.maxValue = _maxHealth;           
                _healthSlider.value = _maxHealth;
            }
            if (_healthCanvas != null)
            {
                _healthCanvas.enabled = true;
            }

            for (int i = 0; i < _maxDamageIndicatorPool; i++)
            {
                GameObject indicator = Instantiate(_damageIndicatorPrefab, _damageIndicatorParent);
                indicator.SetActive(false);
                _damageIndicators.TryAdd(indicator, indicator.GetComponent<DamageIndicator>());
            }
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void ShowDamageIndicator(float amount)
        {
            var availableIndicator = _damageIndicators.FirstOrDefault(di => !di.Key.activeInHierarchy);
            if (availableIndicator.Key != null)
            {
                availableIndicator.Value.SetData(amount, 0.25f, 0.5f);
                availableIndicator.Key.SetActive(true);
            }
        }

        private void UpdateHealthBar()
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = _health.GetHealth();
            }
        }

        #endregion

        #region IDamageHandler Implementation

        public void DoDamage(float amount, out bool destroySource, out bool targetDestroyed, Vector3? hitPoint)
        {
            destroySource = false;
            targetDestroyed = false;

            if (_health.IsDead())
            {
                ShowDamageIndicator(amount);
                return;
            }

            _health.ReduceHealth(amount);
            ShowDamageIndicator(amount);
            UpdateHealthBar();

            if (_health.IsDead())
            {
                targetDestroyed = true;

                foreach (GameObject obj in _objectsToActivate)
                {
                    if (obj.TryGetComponent(out ICanActivate activator))
                    {
                        activator.Activate();
                    }
                }

                if (_isBoss)
                {
                    PlayerProgressService.Instance.ExitToMenu();
                }
            }
        }

        public float GetHealth()
        {
            return _health.GetHealth();
        }

        public float GetMaxHealth()
        {
            return _maxHealth;
        }

        #endregion
    }
}