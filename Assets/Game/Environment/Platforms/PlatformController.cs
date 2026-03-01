/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace GameCore
{
    public class PlatformController : MonoBehaviour, ICanActivate
    {
        #region Public Properties



        #endregion

        #region Private Properties

        private Transform _transform;
        private Rigidbody _rigidbody;        

        // Waypoints
        [SerializeField] private Transform[] _waypoints;
        private int _currentWaypointIndex = 0;
        private Transform _previousWaypoint;
        private Transform _targetWaypoint;

        // Behaviour
        [SerializeField] private bool _activated = true;
        [SerializeField] private bool _shouldMove = true;
        [SerializeField] private bool _shouldMatchRotation = false;

        // Speeds
        [SerializeField] private float _moveSpeed = 1f;
        private float _elapsedTime = 0;
        private float _timeToWaypoint = 0;        

        #endregion


        #region Unity Flow
        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();

            if (_waypoints == null || _waypoints.Length <= 1)
            {
                RPSLib.Debug.Log("Not enough waypoints to function, forget to add them?", RPSLib.Debug.Style.Warning);
                enabled = false;
                return;
            }

            _previousWaypoint = _waypoints[0];
            _targetWaypoint = _waypoints[1];
        }

        private void FixedUpdate()
        {
            if (_activated)
            {
                _elapsedTime += Time.fixedDeltaTime;

                float elapsedTime = _elapsedTime / _timeToWaypoint;
                elapsedTime = Mathf.SmoothStep(0f, 1f, elapsedTime);

                if (_shouldMove)
                {
                    Vector3 targetPos = Vector3.Slerp(_previousWaypoint.position, _targetWaypoint.position, elapsedTime);

                    Vector3 frameDelta = targetPos - _rigidbody.position;

                    _rigidbody.MovePosition(_rigidbody.position + frameDelta);
                }

                if (_shouldMatchRotation)
                {
                    Quaternion targetRot = Quaternion.Lerp(_previousWaypoint.rotation, _targetWaypoint.rotation, elapsedTime);

                    _rigidbody.MoveRotation(targetRot);
                }

                if (elapsedTime >= 1)
                {
                    TargetNextWaypoint();
                }
            }            
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void TargetNextWaypoint()
        {
            _elapsedTime = 0f;

            _previousWaypoint = _waypoints[_currentWaypointIndex];
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            _targetWaypoint = _waypoints[_currentWaypointIndex];
            _timeToWaypoint = Vector3.Distance(_previousWaypoint.position, _targetWaypoint.position) / _moveSpeed;
        }

        #endregion

        #region ICanActivate Implementation

        public void Activate()
        {
            _activated = true;
        }

        #endregion

    }
}