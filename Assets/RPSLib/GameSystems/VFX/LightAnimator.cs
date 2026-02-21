/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(Light))]
    public class LightAnimator : MonoBehaviour
    {

        #region Private Properties

        private Light _light;

        private float _curveStep = 0f;
        private bool _isRunning = false;

        #endregion

        #region Public Properties

        public float CurveTime;
        public bool Loop;
        public AnimationCurve IntensityCurve;
        public Gradient ColorCurve;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void OnEnable()
        {
            _curveStep = 0;

            _isRunning = true;
        }

        private void OnDisable()
        {
            _isRunning = false;
        }

        private void LateUpdate()
        {
            if (!_isRunning)
            {
                return;
            }

            _curveStep += Time.deltaTime;

            float t = Mathf.Clamp01(_curveStep / CurveTime);

            _light.intensity = IntensityCurve.Evaluate(t);
            _light.color = ColorCurve.Evaluate(t);

            if (Loop && _curveStep >= CurveTime)
            {
                _curveStep = 0;
            }
        }

        #endregion

    }
}