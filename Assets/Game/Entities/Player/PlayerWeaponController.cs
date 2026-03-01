/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class PlayerWeaponController : MonoBehaviour
    {
        #region Public Properties



        #endregion

        #region Private Properties

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _bulletPoolSize = 8;
        [SerializeField] private Transform _bulletSpawnPoint;

        [SerializeField] private float _bulletFireRatePerSec = 0.5f;
        private float _nextFireTime = 0f;


        private Dictionary<GameObject, Bullet> _bulletPool = new();

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

        #endregion

        #region Public Methods

        public void HandleShooting()
        {
            _nextFireTime = Time.time + (1f / _bulletFireRatePerSec);

            if (Time.time >= _nextFireTime)
            {
                ShootBullet();
                _nextFireTime = 0f;
            }
        }

        #endregion

        #region Private Methods

        private void ShootBullet()
        {
            foreach (var kvp in _bulletPool)
            {
                if (!kvp.Key.activeInHierarchy)
                {
                    kvp.Key.transform.SetPositionAndRotation(_bulletSpawnPoint.transform.position, _bulletSpawnPoint.transform.rotation);
                    kvp.Key.SetActive(true);
                    kvp.Value.Init();
                }
            }
        }

        #endregion

    }
}