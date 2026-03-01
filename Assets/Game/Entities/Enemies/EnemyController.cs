/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore
{
    public class EnemyController : MonoBehaviour, IDamageHandler
    {
        #region Public Properties



        #endregion

        #region Private Properties

        [SerializeField] private float _maxHealth = 100f;
        private Health _health;

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

        #endregion

        #region IDamageHandler Implementation

        public void DoDamage(float amount, out bool destroySource, out bool targetDestroyed, Vector3? hitPoint)
        {
            destroySource = false;
            targetDestroyed = false;

            if (_health.IsDead())
            {
                return;
            }

            _health.ReduceHealth(amount);
            ShowDamageIndicator(amount);
            RPSLib.Debug.Log($"{gameObject.name} health is now {_health.GetHealth()}", RPSLib.Debug.Style.Warning);

            if (_health.IsDead())
            {
                targetDestroyed = true;
                RPSLib.Debug.Log($"{gameObject.name} is now dead!", RPSLib.Debug.Style.Error);

                foreach (GameObject obj in _objectsToActivate)
                {
                    if (obj.TryGetComponent<ICanActivate>(out ICanActivate activator))
                    {
                        activator.Activate();
                    }                    
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