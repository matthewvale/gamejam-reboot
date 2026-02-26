
using UnityEngine;

/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------
namespace GameCore
{
    public class DraggableObject : Collectable
    {
        #region Public Properties

        public bool SingleUse = false;
        public override bool IsSingleUse
        {
            get => SingleUse;
            set => SingleUse = value;
        }

        public bool Draggable = true;
        public override bool IsDraggable
        {
            get => Draggable;
            set => Draggable = value;
        }

        #endregion

        #region Private Properties

        private bool _isDragging = false;
        private Transform _targetTransform;

        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private float _dragSpeed = 1f;
        [SerializeField] private float _maxDragVelocity = 2f;
        [SerializeField] private float _dragDistanceMin = 0.5f;

        #endregion


        #region Unity Flow

        private void FixedUpdate()
        {
            if (_isDragging && _targetTransform != null)
            {
                Vector3 direction = _targetTransform.position - _rigidBody.position;
                float distanceMin = direction.magnitude;

                if (distanceMin > _dragDistanceMin)
                {
                    direction.Normalize();
                    _rigidBody.AddForce(direction * _dragSpeed, ForceMode.Acceleration);
                    if (_rigidBody.linearVelocity.magnitude > _maxDragVelocity)
                    {
                        _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _maxDragVelocity;
                    }
                }
            }
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion

        #region IInteractable Interface Methods

        public override string GetInteractionText()
        {
            return "Drag";
        }

        public override string GetItemName()
        {
            return CollectableName;
        }

        public override void StartInteraction(Transform source)
        {
            _targetTransform = source;
            _isDragging = true;
        }

        public override void StopInteraction()
        {
            _isDragging = false;
            _targetTransform = null;
        }

        #endregion
    }
}