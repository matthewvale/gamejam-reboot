/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public class LookAt : MonoBehaviour
    {
        public static LookAt Instance { get; private set; }

        #region Private Properties

        private Transform _transform;
        private Transform _targetTransform;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            _transform = transform;
        }

        private void OnDisable()
        {
            _targetTransform = null;
        }

        private void LateUpdate()
        {
            if (_targetTransform != null)
            {
                _transform.LookAt(_targetTransform);
            }
        }

        #endregion

        #region Public Methods

        public void LookAtMe(Transform targetT)
        {
            _targetTransform = targetT;
        }

        #endregion

    }
}