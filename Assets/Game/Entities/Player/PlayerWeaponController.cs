/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
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
        private CameraControllerV2 _cameraController;

        [SerializeField] private Transform _weaponTransform;
        [SerializeField] private AudioSource _weaponAudioSource;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _bulletPoolSize = 8;
        [SerializeField] private Transform _bulletSpawnPoint;

        [SerializeField] private float _bulletForce = 10f;
        [SerializeField] private float _bulletFireRatePerSec = 0.5f;
        private float _nextFireTime = 0f;

        [SerializeField] private Collider _colliderToIgnore;

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

        private void Start()
        {
            _cameraController = CameraControllerV2.Instance;
        }

        private void Update()
        {
            if (_cameraController != null)
            {
                Quaternion targetRot = _cameraController.transform.rotation;
                targetRot.z = 0f;
                _weaponTransform.rotation = targetRot;
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
                    kvp.Key.SetActive(true);
                    kvp.Key.transform.SetPositionAndRotation(_bulletSpawnPoint.transform.position, _bulletSpawnPoint.transform.rotation);
                    Physics.IgnoreCollision(_colliderToIgnore, kvp.Value.GetComponent<Collider>());
                    kvp.Value.Init(_weaponTransform.forward, _bulletForce);
                    _weaponAudioSource.Play();
                    return;
                }
            }
        }

        #endregion

    }
}