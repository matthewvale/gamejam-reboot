/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameCore
{
    public class PlayerProgressService : MonoBehaviour
    {
        public static PlayerProgressService Instance { get; private set; }

        #region Public Properties



        #endregion

        #region Private Properties

        private List<BodyPart> _bodyParts = new();


        [SerializeField] private Canvas _itemCollectedCanvas;
        [SerializeField] private CanvasGroup _itemCollectedCanvasGroup;
        [SerializeField] private TextMeshProUGUI _itemCollectedName;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            _itemCollectedCanvasGroup.alpha = 0f;
        }

        #endregion

        #region Public Methods

        public void AddBodyPart(BodyPart bodyPart)
        {
            if (!_bodyParts.Contains(bodyPart))
            {
                _bodyParts.Add(bodyPart);
                ShowItemCollectedToast(bodyPart);
            }
        }

        #endregion

        #region Private Methods

        private void ShowItemCollectedToast(Collectable itemCollected)
        {
            _itemCollectedName.text = itemCollected.CollectableName;
            LeanTween.alphaCanvas(_itemCollectedCanvasGroup, 1f, 0.5f).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(_itemCollectedCanvasGroup, 0f, 0.5f).setDelay(2f);
            });
        }

        #endregion

    }
}