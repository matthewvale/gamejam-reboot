/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;

namespace RPSCore
{
    public class SceneFadeService : MonoBehaviour
    {
        #region Instancing

        public static SceneFadeService Instance { get; set; }

        #endregion

        #region Private Properties

        #endregion

        #region Public Properties

        public CanvasGroup CanvasGroup;
        public TextMeshProUGUI ForegroundText;

        #endregion


        #region Unity Flow

        protected void Awake()
        {
            Instance = this;
            CanvasGroup.alpha = 0f;
        }

        #endregion

        #region Public Methods

        public void TriggerFade(string mainText, float fadeSpeed, float duration, float startIn = 0f)
        {
            ForegroundText.text = mainText;
            CanvasGroup.alpha = 0f;

            LeanTween.alphaCanvas(CanvasGroup, 1f, fadeSpeed).setDelay(startIn);
            LeanTween.alphaCanvas(CanvasGroup, 0f, fadeSpeed).setDelay(startIn + fadeSpeed + duration);
        }

        #endregion

    }

}