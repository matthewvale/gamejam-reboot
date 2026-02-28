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

        public void Init()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.forward, ForceMode.Impulse);
        }

        #endregion

        #region Private Methods

        #endregion

        #region BulletBase Implementation

        public override float Damage { get; set; }

        public override void OnCollisionEnter(Collision collisionData)
        {
            if (collisionData.gameObject.TryGetComponent<IDamageHandler>(out var damageHandler))
            {
                damageHandler.DoDamage(Damage, out _, out _, null);
                gameObject.SetActive(false);
            }
        }

        public override void OnTriggerEnter(Collider collisionData)
        {

        }

        #endregion
    }
}