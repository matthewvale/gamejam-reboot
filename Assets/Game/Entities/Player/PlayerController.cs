/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections.Generic;
using System.Linq;
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
        private InputAction _interactAction;

        private CameraControllerV2 _camera;
        private Transform _transform;
        private Rigidbody _rb;

        private bool _isGrounded = false;

        [SerializeField] private float _groundCheckDistance = 1.05f;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
        [SerializeField] private LayerMask _groundMask;

        // Interactions
        [SerializeField] private Canvas _interactableCanvas;
        [SerializeField] private RectTransform _interactablePromptTransform;
        [SerializeField] private SphereCollider _interactionSphereCollider;
        [SerializeField] private float _interactionRadius = 2f;
        private Dictionary<GameObject, IInteractable> _interactablesInRange = new();
        private IInteractable _nearestInteractable;
        private Transform _interactableTransform;

        #endregion


        #region Unity Flow

        private void Start()
        {
            _transform = transform;
            _camera = CameraControllerV2.Instance;
            _camera.StartFollowingObject(transform);
            _rb = GetComponent<Rigidbody>();
            _interactionSphereCollider.radius = _interactionRadius;

            _moveAction = InputSystem.actions.FindAction(InputActionConstants.DirectionalMove);
            _jumpAction = InputSystem.actions.FindAction(InputActionConstants.Space);
            _interactAction = InputSystem.actions.FindAction(InputActionConstants.InteractKey);

            _jumpAction.performed += Jump;
            _interactAction.performed += ctx => ProcessInteraction();

            UpdateInteractionPrompt(false);
        }

        private void OnDestroy()
        {
            _jumpAction.performed -= Jump;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void FixedUpdate()
        {
            // Check if grounded
            Debug.DrawRay(_transform.position, Vector3.down * _groundCheckDistance, Color.red);
            _isGrounded = Physics.Raycast(_transform.position, Vector3.down, _groundCheckDistance, _groundMask);
        }

        private void LateUpdate()
        {
            if (_interactableTransform && _interactableCanvas.enabled)
            {
                // Convert world position of interactable to screen point and set                
                _interactablePromptTransform.position = _camera.GetCameraComponent().WorldToScreenPoint(_interactableTransform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                if (other.TryGetComponent(out IInteractable interactable))
                {
                    if (_interactablesInRange.TryAdd(other.gameObject, interactable))
                    {
                        ToggleNearestInteractablePrompt();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Interactable"))
            {
                foreach (KeyValuePair<GameObject, IInteractable> interactableObject in _interactablesInRange)
                {
                    if (interactableObject.Key == other.gameObject)
                    {
                        _interactablesInRange.Remove(interactableObject.Key);
                        UpdateInteractionPrompt(false);
                        ToggleNearestInteractablePrompt();
                        break;
                    }
                }
            }
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void HandleMovement()
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
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                _rb.MoveRotation(targetRotation);
            }                        
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }
        }

        private void ToggleNearestInteractablePrompt()
        {
            _nearestInteractable = null;
            if (_interactablesInRange.Count > 0)
            {
                // Get closest interactable and toggle its prompt
                float smallestDistance = Mathf.Infinity;
                foreach (var interactcable in _interactablesInRange)
                {
                    float distance = Vector3.Distance(interactcable.Key.transform.position, _transform.position);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        _nearestInteractable = interactcable.Value;
                    }
                }
            }

            if (_nearestInteractable != null)
            {
                UpdateInteractionPrompt(true);
            }
            else
            {
                UpdateInteractionPrompt(false);
            }
        }

        private void ProcessInteraction()
        {
            _nearestInteractable?.Interact();
            _nearestInteractable = null;
            UpdateInteractionPrompt(false);
        }

        private void UpdateInteractionPrompt(bool state)
        {
            if (!state)
            {
                _interactableCanvas.enabled = false;
                return;
            }

            _interactableCanvas.enabled = true;
            if (_interactablesInRange.Count > 0)
            {
                _interactableTransform = _interactablesInRange.FirstOrDefault(x => x.Value == _nearestInteractable).Key.transform;
            }            
        }

        #endregion

    }
}