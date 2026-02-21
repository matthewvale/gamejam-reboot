/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private PlayerController _playerController;

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
                CheckForAbilityUnlocks(bodyPart);
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

        private void CheckForAbilityUnlocks(BodyPart bodyPart)
        {
            switch (bodyPart.Type)
            {
                case BodyPart.BodyPartType.Leg:
                    if (_bodyParts.Count(bp => bp.Type == BodyPart.BodyPartType.Leg) >= 2)
                    {
                        _playerController.CanJump = true;
                    }
                    break;
                case BodyPart.BodyPartType.Arm:
                    // Future ability unlocks for arms would go here
                    break;
                case BodyPart.BodyPartType.Head:
                    // Bring colour back into the environment
                    break;
                case BodyPart.BodyPartType.Laser:
                    _playerController.CanShootLaser = true;
                    break;
            }
        }

        #endregion

    }
}