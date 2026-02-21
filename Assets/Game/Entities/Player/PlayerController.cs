/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        #region Public Properties



        #endregion

        #region Private Properties

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private CameraControllerV2 _camera;
        private Transform _transform;
        private Rigidbody _rb;

        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
        [SerializeField] private LayerMask _groundMask;

        private bool _isGrounded = false;

        #endregion


        #region Unity Flow

        private void Start()
        {
            _transform = transform;
            _camera = CameraControllerV2.Instance;
            _camera.StartFollowingObject(transform);
            _rb = GetComponent<Rigidbody>();

            _moveAction = InputSystem.actions.FindAction(InputActionConstants.DirectionalMove);
            _jumpAction = InputSystem.actions.FindAction(InputActionConstants.Space);

            _jumpAction.performed += Jump;
        }

        private void OnDestroy()
        {
            _jumpAction.performed -= Jump;
        }

        private void Update()
        {
            var moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 camForward = _camera.GetPivotTransform().forward;
            Vector3 camRight = _camera.GetPivotTransform().right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            // Move direction, camera relative
            Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;
            _rb.MovePosition(_rb.position + moveDirection * _moveSpeed * Time.fixedDeltaTime);

            // Rotation is handled by the camera, so we just need to ensure the rigidbody's rotation matches the transform's rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            _rb.MoveRotation(targetRotation);
        }

        private void FixedUpdate()
        {
            // Check if grounded
            Debug.DrawRay(_transform.position, Vector3.down * 1.1f, Color.red);
            _isGrounded = Physics.Raycast(_transform.position, Vector3.down, 1.1f, _groundMask);
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void Jump(InputAction.CallbackContext context)
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }            
        }

        #endregion

    }
}