/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;
using UnityEngine.InputSystem;

namespace RPSCore
{

    public class MiniMapService : MonoBehaviour
    {
        public static MiniMapService Instance { get; private set; }


        #region Private Properties

        private Transform _camTransform;
        private Transform _target;
        private Transform _targetCameraTransform;

        private InputAction ZoomAction;
        private const float ZoomLerpSpeed = 5f;
        private const float ZoomButtonStep = 20f;
        private const float ZoomKeyPressSpeed = 150f;

        #endregion

        #region Public Properties

        public Camera Camera;
        public Camera FullScreenCamera;
        public bool RotateWithCamera = true;
        public float CameraSmoothSpeed = 1f;
        public float HeightAboveTarget = 50f;

        public float OrthographicSize = 50f;
        public float MinZoom = 20f;
        public float MaxZoom = 100f;

        [Header("UI Elements")]
        public Canvas MiniMapCanvas;
        public GameObject MiniMapSmallContainer;
        public GameObject MiniMapLargeContainer;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            Camera.orthographicSize = OrthographicSize;
            _camTransform = Camera.transform;

            FullScreenCamera.enabled = false;

            ZoomAction = InputSystem.actions.FindAction(InputActionConstants.MiniMapZoom);
        }

        private void LateUpdate()
        {
            if (_target != null)
            {
                Vector3 targetPosition = _target.position;
                targetPosition.y += HeightAboveTarget;
                _camTransform.position = targetPosition;
            }

            if (RotateWithCamera)
            {
                Vector3 curRotation = _camTransform.eulerAngles;
                curRotation.y = _targetCameraTransform.eulerAngles.y;
                _camTransform.eulerAngles = curRotation;
            }

            // Zoom controls
            float zoomValue = ZoomAction.ReadValue<float>();
            if (zoomValue != 0)
            {
                ZoomSmooth(zoomValue);
            }

            if (Camera.orthographicSize != OrthographicSize)
            {
                OrthographicSize = Mathf.Clamp(OrthographicSize, MinZoom, MaxZoom);

                Camera.orthographicSize = Mathf.Lerp(
                    Camera.orthographicSize,
                    OrthographicSize,
                    Time.unscaledDeltaTime * ZoomLerpSpeed
                );
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public void FollowTarget(Transform target, Transform targetCam)
        {
            _target = target;
            _targetCameraTransform = targetCam;
        }

        public void ZoomStep(float delta)
        {
            OrthographicSize -= delta * ZoomButtonStep;
        }

        public void ZoomSmooth(float delta)
        {
            OrthographicSize -= delta * ZoomKeyPressSpeed * Time.unscaledDeltaTime;
        }

        public void ToggleMiniMap()
        {
            if (MiniMapSmallContainer.activeInHierarchy)
            {
                MiniMapSmallContainer.SetActive(false);

                Camera.enabled = false;
                FullScreenCamera.enabled = true;

                MiniMapLargeContainer.SetActive(true);
            }
            else
            {                
                MiniMapLargeContainer.SetActive(false);

                FullScreenCamera.enabled = false;
                Camera.enabled = true;

                MiniMapSmallContainer.SetActive(true);
            }
        }

        #endregion

    }
}