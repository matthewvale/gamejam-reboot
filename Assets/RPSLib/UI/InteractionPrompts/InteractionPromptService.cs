/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{

    public class InteractionPromptService : MonoBehaviour
    {
        #region Instancing

        public static InteractionPromptService Instance { get; private set; }

        #endregion

        #region Private Properties


        #endregion

        #region Public Properties

        public Canvas UIPromptCanvas;
        public TextMeshProUGUI UIPromptText;

        public Image PromptIcon;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            HideInteractionPrompt();
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public void ShowInteractionPrompt(string message)
        {
            UIPromptText.text = message;
            UIPromptCanvas.enabled = true;
        }

        public void HideInteractionPrompt()
        {
            UIPromptCanvas.enabled = false;
        }

        #endregion

    }
}