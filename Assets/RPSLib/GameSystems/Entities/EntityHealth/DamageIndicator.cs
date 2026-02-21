/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;

namespace RPSCore
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DamageIndicator : MonoBehaviour
    {
        #region Public Properties


        #endregion

        #region Private Properties

        private Transform _transform;
        private CanvasGroup _canvasGroup;

        [SerializeField] private TextMeshProUGUI _damageText;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _transform = transform;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _transform.SetAsLastSibling();
        }

        private void OnDisable()
        {

        }

        #endregion

        #region Public Methods

        public void SetData(float amount, float startHide, float hide)
        {
            _damageText.SetText(amount.ToString());
            _transform.localPosition = Vector3.zero;

            _canvasGroup.alpha = 1f;
            LeanTween.cancel(gameObject);
            LeanTween.moveLocalY(gameObject, _transform.localPosition.y + 40f, hide).setEaseOutCubic();
            LeanTween.alphaCanvas(_canvasGroup, 0f, hide - startHide).setDelay(startHide);
            Invoke(nameof(DisableIndicator), hide);
        }

        #endregion

        #region Private Methods

        private void DisableIndicator()
        {
            gameObject.SetActive(false);
        }

        #endregion

    }
}