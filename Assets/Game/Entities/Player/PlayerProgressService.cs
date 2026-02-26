/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

        [SerializeField] private List<GameObject> _levelAreaParents;
        private int _currentAreaIndex = 0;
        private WaitForSeconds _levelAreaLoadTime = new(3f);
        //private Coroutine _levelAreaLoadCoroutine;

        [SerializeField] private PlayerController _playerController;

        [SerializeField] private Volume _postProcessingVolume;
        //private VolumeParameter<float> _colourSaturationParameter;
        private ColorAdjustments _colorAdjustments;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;

            _itemCollectedCanvasGroup.alpha = 0f;
            _postProcessingVolume.profile.TryGet(out _colorAdjustments);
            _levelAreaParents[0].SetActive(true);
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
                LoadNextArea();
            }
        }

        public void LoadNextArea()
        {
            //_levelAreaLoadCoroutine = 
            StartCoroutine(LoadLevel());
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
            bool bothArmsFound = false;
            bool bothLegsFound = false;

            switch (bodyPart.Type)
            {
                case BodyPart.BodyPartType.LegL:
                    bothLegsFound = _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.LegL) &&
                                         _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.LegR);
                    _playerController.CanJump = bothLegsFound;
                    break;

                case BodyPart.BodyPartType.LegR:
                    bothLegsFound = _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.LegL) &&
                                         _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.LegR);
                    _playerController.CanJump = bothLegsFound;
                    break;

                case BodyPart.BodyPartType.ArmL:
                    _playerController.CanDrag = true;
                    bothArmsFound = _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.ArmL) &&
                                        _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.ArmR);
                    if (bothArmsFound)
                    {
                        _playerController.DragSpeed = 2f;
                    }
                    break;

                case BodyPart.BodyPartType.ArmR:
                    _playerController.CanDrag = true;
                    bothArmsFound = _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.ArmL) &&
                                        _bodyParts.Any(bp => bp.Type == BodyPart.BodyPartType.ArmR);
                    if (bothArmsFound)
                    {
                        _playerController.DragSpeed = 2f;
                    }
                    break;

                case BodyPart.BodyPartType.Head:
                    // Bring colour back into the environment
                    break;

                case BodyPart.BodyPartType.Laser:
                    _playerController.CanShootLaser = true;
                    break;
            }
        }

        private IEnumerator LoadLevel()
        {
            SceneFadeService.Instance.TriggerFade($"Re-attaching part: {_bodyParts[^1].Type}", 2f, 3f);
            yield return _levelAreaLoadTime;

            _currentAreaIndex++;
            for (int i = 0; i < _levelAreaParents.Count; i++)
            {
                _levelAreaParents[i].SetActive(i == _currentAreaIndex);
            }

            _playerController.EnableBodyPart(_bodyParts[^1].Type);
            _playerController.SetHappyState(_currentAreaIndex);
            _playerController.ResetPosition();

            _colorAdjustments.saturation.value += 200 / _levelAreaParents.Count;
        }

        #endregion

    }
}