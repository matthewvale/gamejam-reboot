/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageHandler
    {
        #region Public Properties

        public bool CanJump { get; set; } = false;
        public bool CanDoubleJump { get; set; } = false;
        public bool CanDrag { get; set; } = false;
        public float DragSpeed { get; set; } = 0.5f;
        public bool CanShootLaser { get; set; } = false;

        public bool UnlockAllProgress = false;

        #endregion

        #region Private Properties

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;
        private InputAction _shootAction;
        private InputAction _escapeAction;

        private CameraControllerV2 _camera;
        private Transform _transform;
        private Rigidbody _rigidBody;

        private bool _isGrounded = false;
        private Vector2 _moveInput;
        private Vector3 _camForward;
        private Vector3 _camRight;
        private Vector3 _inputDirection;
        private bool _onPlatform = false;

        // Health
        private Health _health;
        [SerializeField] private float _maxHealth = 100f;

        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private float _groundCheckDistance = 1.05f;
        [SerializeField] private float _jumpPower = 10f;
        private float _currentJumpPower = 0f;
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
        private int _legCount = 0;

        // Happy state visualizer
        [SerializeField] private GameObject[] _happyStates;

        // Weapons
        [SerializeField] private PlayerWeaponController _weaponController;
        private bool _isShooting = false;

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
            _escapeAction = InputSystem.actions.FindAction(InputActionConstants.Escape);
            _escapeAction.started -= LoadMainMenu;
            _escapeAction.started += LoadMainMenu;

            _jumpAction.performed += Jump;
            _interactAction.started += ctx => StartInteraction();
            _interactAction.canceled += ctx => StopInteraction();

            _shootAction = InputSystem.actions.FindAction(InputActionConstants.LMB);
            _shootAction.started += ctx => StartShooting();
            _shootAction.canceled += ctx => StopShooting();

            if (UnlockAllProgress)
            {
                UnlockAll();
                UpdateInteractionPrompt(false);
                return;
            }

            _health = new Health();
            _health.SetHealth(_maxHealth);

            DisableAllBodyParts();
            SetHappyState(0);
            UpdateInteractionPrompt(false);

            CursorService.Instance.SetCursorType(CursorService.CursorType.DEFEND, CursorLockMode.Locked);
        }

        private void OnEnable()
        {
            _rigidBody.position = _playerSpawnPoint.position;
        }

        private void OnDestroy()
        {
            _escapeAction.started -= LoadMainMenu;
            _jumpAction.performed -= Jump;
            _interactAction.started -= ctx => StartInteraction();
            _interactAction.canceled -= ctx => StopInteraction();
            _shootAction.started -= ctx => StartShooting();
            _shootAction.canceled -= ctx => StopShooting();
        }

        private void Update()
        {
            _moveInput = _moveAction.ReadValue<Vector2>();
            _camForward = _camera.GetPivotTransform().forward;
            _camRight = _camera.GetPivotTransform().right;

            _inputDirection = _camForward * _moveInput.y + _camRight * _moveInput.x;
            _inputDirection.y = 0f;
            _inputDirection.Normalize();
        }

        private void FixedUpdate()
        {
            // Check if grounded
            _isGrounded = Physics.Raycast(_transform.position, Vector3.down, _groundCheckDistance, _groundMask);

            if (!_onPlatform)
            {
                HandleMovement();
            }

            if (CanShootLaser && _isShooting)
            {
                _weaponController.HandleShooting();
            }
        }

        private void LateUpdate()
        {
            UpdateNearestInteractablePrompt();
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
                    _interactablesInRange.TryAdd(other.gameObject, interactable);
                }
            }

            if (other.CompareTag("Respawn"))
            {
                _rigidBody.position = _playerSpawnPoint.position;
                _rigidBody.linearVelocity = Vector3.zero;
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
                        interactableObject.Value.StopInteraction();
                        _interactablesInRange.Remove(interactableObject.Key);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void EnableBodyPart(BodyPart.BodyPartType partType)
        {
            for (int i = 0; i < _bodyVFX.Length; i++)
            {
                _bodyVFX[i].Stop();
            }

            switch (partType)
            {
                case BodyPart.BodyPartType.LegL:
                    _legL.SetActive(true);
                    _boxCollider.size = _fullBodyColliderSize;
                    break;
                case BodyPart.BodyPartType.LegR:
                    _legR.SetActive(true);
                    _boxCollider.size = _fullBodyColliderSize;
                    break;
                case BodyPart.BodyPartType.ArmL:
                    CanDrag = true;
                    _armL.SetActive(true);
                    break;
                case BodyPart.BodyPartType.ArmR:
                    CanDrag = true;
                    _armR.SetActive(true);
                    break;
                case BodyPart.BodyPartType.Head:
                    _head.SetActive(true);
                    break;
                case BodyPart.BodyPartType.Laser:
                    CanShootLaser = true;
                    _laser.SetActive(true);
                    CursorService.Instance.SetCursorType(CursorService.CursorType.ATTACK, CursorLockMode.Locked);
                    break;
            }
        }

        public void ResetPosition()
        {
            _interactablesInRange.Clear();
            _rigidBody.position = _playerSpawnPoint.position;
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }

        public void SetHappyState(int index)
        {
            for (int i = 0; i < _happyStates.Length; i++)
            {
                _happyStates[i].SetActive(i == index);
            }
        }

        public void SetCanJump()
        {
            CanJump = true;
            _legCount++;
            if (_legCount == 1)
            {
                _currentJumpPower = _jumpPower / 1.1f;
            }
            else
            {
                _currentJumpPower = _jumpPower;
            }
        }

        public void UnlockAll()
        {
            EnableBodyPart(BodyPart.BodyPartType.LegR);
            EnableBodyPart(BodyPart.BodyPartType.LegL);
            EnableBodyPart(BodyPart.BodyPartType.ArmL);
            EnableBodyPart(BodyPart.BodyPartType.ArmR);
            EnableBodyPart(BodyPart.BodyPartType.Laser);
            EnableBodyPart(BodyPart.BodyPartType.Head);
            SetCanJump();
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
            _laser.SetActive(false);
        }

        private void HandleMovement()
        {
            // Move direction, camera relative
            var moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 camForward = _camera.GetPivotTransform().forward;
            Vector3 camRight = _camera.GetPivotTransform().right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * moveInput.y + camRight * moveInput.x;
            moveDirection.Normalize();

            Vector3 targetVel = moveDirection * _moveSpeed;

            _rigidBody.MovePosition(_rigidBody.position + targetVel * _moveSpeed * Time.fixedDeltaTime);

            // Rotation is handled by the camera, so we just need to ensure the rigidbody's rotation matches the transform's rotation
            if (_inputDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_inputDirection, Vector3.up);

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
                _rigidBody.AddForce(Vector3.up * _currentJumpPower, ForceMode.Impulse);
            }
        }

        private void UpdateNearestInteractablePrompt()
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
            if (!state || _nearestInteractable == null)
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
            }
        }

        private void StopInteraction()
        {
            _currentlySelectedInteractable?.StopInteraction();
            _currentlySelectedInteractable = null;
        }

        private void StartShooting()
        {
            _isShooting = true;
        }

        private void StopShooting()
        {
            _isShooting = false;
        }

        private void LoadMainMenu(InputAction.CallbackContext context)
        {
            _escapeAction.started -= LoadMainMenu;
            CursorService.Instance.SetCursorType(CursorService.CursorType.MAIN, CursorLockMode.None);
            PlayerProgressService.Instance.ExitToMenu();
        }

        #endregion

        #region IDamageHandler Implementation

        public void DoDamage(float amount, out bool destroySource, out bool targetDestroyed, Vector3? hitPoint)
        {
            destroySource = false;
            targetDestroyed = false;

            _health.SetHealth(_health.GetHealth() - amount);
            if (_health.IsDead())
            {
                _health.SetHealth(_maxHealth);
                ResetPosition();
            }
        }

        public float GetHealth()
        {
            return _health.GetHealth();
        }

        public float GetMaxHealth()
        {
            return _maxHealth;
        }

        #endregion

    }
}