/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{
    public class UILookAtCamera : MonoBehaviour
    {
        #region Private Properties

        [SerializeField] private CameraControllerV2 _camera;
        private Transform _transform;
        private Quaternion _targetRotation;

        #endregion

        #region Public Properties

        public bool HideAfterDistance = false;
        public float DistanceToHide = 450f;

        public bool RotateOnX = true;
        public bool RotateOnY = true;
        public bool RotateOnZ = false;

        #endregion

        #region Unity Flow

        void OnEnable()
        {
            if (_camera == null)
            {
                if (CameraControllerV2.Instance != null)
                {
                    _camera = CameraControllerV2.Instance;
                }
            }

            _transform = transform;
        }

        private void LateUpdate()
        {
            if (_camera != null && _transform != null)
            {
                LookAtCamera();
            }
        }

        #endregion

        #region Private Methods

        private void LookAtCamera()
        {
            //_targetRotation = _camera.transform.rotation;

            //if (!RotateOnX)
            //{
            //    _targetRotation.x = 0f;
            //}

            //if (!RotateOnY)
            //{
            //    _targetRotation.y = 0f;
            //}

            //if (!RotateOnZ)
            //{
            //    _targetRotation.z = 0f;
            //}

            //_transform.rotation = _targetRotation;

            _transform.forward = _camera.transform.forward;

            if (HideAfterDistance)
            {
                if (RPSLib.Maths.Distances.SqrMagDistance(_transform.position, _camera.transform.position) > DistanceToHide)
                {
                    _transform.gameObject.SetActive(false);
                }
                else
                {
                    _transform.gameObject.SetActive(true);
                }
            }
        }

        #endregion

        #region Public Methods

        #endregion

    }
}