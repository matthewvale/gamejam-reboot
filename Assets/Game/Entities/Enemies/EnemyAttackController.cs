/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class EnemyAttackController : MonoBehaviour
    {
        #region Public Properties



        #endregion

        #region Private Properties

        [SerializeField] private Transform _playerTransform;

        //[SerializeField] private Transform _weaponTransform;
        [SerializeField] private AudioSource _weaponAudioSource;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _bulletPoolSize = 8;
        [SerializeField] private Transform _bulletSpawnPoint;

        [SerializeField] private float _bulletForce = 10f;
        //[SerializeField] private float _bulletFireRatePerSec = 0.5f;

        private Dictionary<GameObject, Bullet> _bulletPool = new();

        [SerializeField] private Collider[] _collidersToIgnore;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            for (int i = 0; i < _bulletPoolSize; i++)
            {
                GameObject bullet = Instantiate(_bulletPrefab);
                bullet.SetActive(false);
                _bulletPool.TryAdd(bullet, bullet.GetComponent<Bullet>());
            }
        }

        private void OnEnable()
        {
            CancelInvoke();
            Invoke(nameof(Attack), 5f);
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void Attack()
        {
            foreach (var kvp in _bulletPool)
            {
                if (!kvp.Key.activeInHierarchy)
                {
                    kvp.Key.SetActive(true);

                    Vector3 directionToPlayer = (_playerTransform.position - _bulletSpawnPoint.position).normalized;
                    kvp.Key.transform.SetPositionAndRotation(_bulletSpawnPoint.position, Quaternion.LookRotation(directionToPlayer));
                    foreach (var col in _collidersToIgnore)
                    {
                        Physics.IgnoreCollision(col, kvp.Value.GetComponent<Collider>());
                    }
                    kvp.Value.Init(directionToPlayer, _bulletForce);
                    _weaponAudioSource.Play();
                    Invoke(nameof(Attack), Random.Range(2, 4f));
                    return;
                }
            }
        }

        #endregion

    }
}