/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : BulletBase
    {
        #region Public Properties



        #endregion

        #region Private Properties

        private Transform _transform;
        private Rigidbody _rigidbody;

        [SerializeField] private float _damage = 10f;

        [SerializeField] private GameObject _bulletImpactVFX;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            Damage = _damage;
        }

        #endregion

        #region Public Methods

        public void Init(Vector3 forceDirection, float force)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(forceDirection * force, ForceMode.Impulse);
            CancelInvoke();
            Invoke(nameof(DisableBullet), 3f);
        }

        #endregion

        #region Private Methods

        private void DisableBullet()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region BulletBase Implementation

        public override float Damage { get; set; }

        public override void OnTriggerEnter(Collider collisionData)
        {
            if (collisionData.gameObject.TryGetComponent<IDamageHandler>(out var damageHandler))
            {
                damageHandler.DoDamage(Damage, out _, out _, null);
                _bulletImpactVFX.transform.position = _transform.position;
                _bulletImpactVFX.transform.SetParent(null);
                _bulletImpactVFX.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        public override void OnCollisionEnter(Collision collisionData)
        {
        }

        #endregion
    }
}