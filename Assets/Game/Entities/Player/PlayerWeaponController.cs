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

        private Transform _transform;

        [SerializeField] private AudioSource _weaponAudioSource;
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
            _transform = transform;

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
            _nextFireTime += Time.deltaTime;
            float _calculatedFiringSpeed = 1f / _bulletFireRatePerSec;
            if (_nextFireTime >= _calculatedFiringSpeed)
            {
                _nextFireTime = 0;
                ShootBullet();
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
                    kvp.Value.Init(_transform.forward);
                    _weaponAudioSource.Play();
                }
            }
        }

        #endregion

    }
}