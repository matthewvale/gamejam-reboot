/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameCore.BodyPart;

namespace GameCore
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        #region Public Properties

        public bool CanJump { get; set; } = false;
        public bool CanDoubleJump { get; set; } = false;
        public bool CanDrag { get; set; } = false;
        public float DragSpeed { get; set; } = 0.5f;
        public bool CanShootLaser { get; set; } = false;

        #endregion

        #region Private Properties

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;

        private CameraControllerV2 _camera;
        private Transform _transform;
        private Rigidbody _rigidBody;

        private bool _isGrounded = false;

        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private float _groundCheckDistance = 1.05f;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _rotationSpeed = 1f;
        [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
        [SerializeField] private LayerMask _groundMask;

        // Interactions
        [SerializeField] private Canvas _interactableCanvas;
        [SerializeField] private RectTransform _interactablePromptTransform;
        [SerializeField] private TextMeshProUGUI _interactablePromptType;
        [SerializeField] private TextMeshProUGUI _interactablePromptText;
        [SerializeField] private SphereCollider _interactionSphereCollider;
        [SerializeField] private float _interactionRadius = 2f;
        private Dictionary<GameObject, IInteractable> _interactablesInRange = new();
        private IInteractable _nearestInteractable;
        private IInteractable _currentlySelectedInteractable;
        private Transform _interactableTransform;

        // Body part management
        [SerializeField] private ParticleSystem[] _bodyVFX;
        [SerializeField] private GameObject _legL;
        [SerializeField] private GameObject _legR;
        [SerializeField] private GameObject _armL;
        [SerializeField] private GameObject _armR;
        [SerializeField] private GameObject _head;
        [SerializeField] private GameObject _laser;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private Vector3 _fullBodyColliderSize = new(1f, 2f, 0.5f);

        // Happy state visualizer
        [SerializeField] private GameObject[] _happyStates;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _transform = transform;
            _camera = CameraControllerV2.Instance;
            _camera.StartFollowingObject(transform);
            _rigidBody = GetComponent<Rigidbody>();
            _interactionSphereCollider.radius = _interactionRadius;

            _moveAction = InputSystem.actions.FindAction(InputActionConstants.DirectionalMove);
            _jumpAction = InputSystem.actions.FindAction(InputActionConstants.Space);
            _interactAction = InputSystem.actions.FindAction(InputActionConstants.InteractKey);

            _jumpAction.performed += Jump;
            _interactAction.started += ctx => StartInteraction();
            _interactAction.canceled += ctx => StopInteraction();

            DisableAllBodyParts();
            SetHappyState(0);
            UpdateInteractionPrompt(false);

            CursorService.Instance.SetCursorType(CursorService.CursorType.DEFEND, CursorLockMode.Locked);
        }

        private void OnEnable()
        {
            // This is not how I would usually do it, but for the sake of time...
            _rigidBody.position = _playerSpawnPoint.position;
        }

        private void OnDestroy()
        {
            _jumpAction.performed -= Jump;
        }

        private void FixedUpdate()
        {
            // Check if grounded
            Debug.DrawRay(_transform.position, Vector3.down * _groundCheckDistance, Color.red);
            _isGrounded = Physics.Raycast(_transform.position, Vector3.down, _groundCheckDistance, _groundMask);

            HandleMovement();
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

            if (other.CompareTag("Respawn"))
            {
                _rigidBody.position = _playerSpawnPoint.position;
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

        public void EnableBodyPart(BodyPartType partType)
        {
            for (int i = 0; i < _bodyVFX.Length; i++)
            {
                _bodyVFX[i].Stop();
            }

            switch (partType)
            {
                case BodyPartType.LegL:
                    _legL.SetActive(true);
                    _boxCollider.size = _fullBodyColliderSize;
                    break;
                case BodyPartType.LegR:
                    _legR.SetActive(true);
                    _boxCollider.size = _fullBodyColliderSize;
                    break;
                case BodyPartType.ArmL:
                    CanDrag = true;
                    _armL.SetActive(true);
                    break;
                case BodyPartType.ArmR:
                    CanDrag = true;
                    _armR.SetActive(true);
                    break;
                case BodyPartType.Head:
                    _head.SetActive(true);
                    break;
                case BodyPartType.Laser:
                    CanShootLaser = true;
                    _laser.SetActive(true);
                    CursorService.Instance.SetCursorType(CursorService.CursorType.ATTACK, CursorLockMode.Locked);
                    break;
            }
        }

        public void ResetPosition()
        {
            _rigidBody.position = _playerSpawnPoint.position;
        }

        public void SetHappyState(int index)
        {
            for (int i = 0; i < _happyStates.Length; i++)
            {
                _happyStates[i].SetActive(i == index);
            }
        }

        #endregion

        #region Private Methods

        private void DisableAllBodyParts()
        {
            _legL.SetActive(false);
            _legR.SetActive(false);
            _armL.SetActive(false);
            _armR.SetActive(false);
            _head.SetActive(false);
            //_laser.SetActive(false);
        }

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
            moveDirection.Normalize();

            Vector3 targetVel = moveDirection * _moveSpeed;

            Vector3 smoothedMove = Vector3.Lerp(
                _rigidBody.linearVelocity,
                targetVel,
                _acceleration * Time.fixedDeltaTime
            );

            //_rb.linearVelocity = smoothedMove;

            _rigidBody.MovePosition(_rigidBody.position + smoothedMove * _moveSpeed * Time.fixedDeltaTime);

            // Rotation is handled by the camera, so we just need to ensure the rigidbody's rotation matches the transform's rotation
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

                Quaternion newRot = Quaternion.Slerp(_rigidBody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);

                _rigidBody.MoveRotation(newRot);
            }
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (!CanJump)
            {
                return;
            }

            if (_isGrounded)
            {
                _rigidBody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            }
        }

        private void ToggleNearestInteractablePrompt()
        {
            _nearestInteractable = null;
            if (_interactablesInRange.Count > 0)
            {
                // Get closest interactable and toggle its prompt
                float smallestDistance = Mathf.Infinity;
                foreach (var interactable in _interactablesInRange)
                {
                    if (interactable.Key == null)
                    {
                        _interactablesInRange.Remove(interactable.Key);
                        continue;
                    }
                    float distance = Vector3.Distance(interactable.Key.transform.position, _transform.position);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        _nearestInteractable = interactable.Value;
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

        private void UpdateInteractionPrompt(bool state)
        {
            if (!state)
            {
                _interactableCanvas.enabled = false;
                return;
            }

            _interactablePromptType.SetText(_nearestInteractable.GetInteractionText());
            _interactablePromptText.SetText(_nearestInteractable.GetItemName());

            _interactableCanvas.enabled = true;
            if (_interactablesInRange.Count > 0)
            {
                _interactableTransform = _interactablesInRange.FirstOrDefault(x => x.Value == _nearestInteractable).Key.transform;
            }
        }

        private void StartInteraction()
        {
            if (_nearestInteractable == null)
            {
                return;
            }

            if (_nearestInteractable.IsDraggable && !CanDrag)
            {
                return;
            }

            _nearestInteractable?.StartInteraction(_transform);
            _currentlySelectedInteractable = _nearestInteractable;

            if (_nearestInteractable.IsSingleUse)
            {
                // Remove the interactable from the _interactablesInRange list
                if (_nearestInteractable != null)
                {
                    _interactablesInRange.Remove(_interactablesInRange.FirstOrDefault(x => x.Value == _nearestInteractable).Key);
                }

                _nearestInteractable = null;
                ToggleNearestInteractablePrompt();
            }
        }

        private void StopInteraction()
        {
            _currentlySelectedInteractable?.StopInteraction();
            _currentlySelectedInteractable = null;
        }

        #endregion

    }
}