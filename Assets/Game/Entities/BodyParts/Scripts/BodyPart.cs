using UnityEngine;

/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------
namespace GameCore
{
    public class BodyPart : Collectable
    {
        #region Public Properties

        public BodyPartType Type;
        public enum BodyPartType
        {
            None,
            LegL,
            LegR,
            ArmL,
            ArmR,
            Head,
            Laser
        }

        public bool SingleUse = false;
        public override bool IsSingleUse
        {
            get => SingleUse;
            set => SingleUse = value;
        }

        public bool Draggable = false;
        public override bool IsDraggable
        {
            get => Draggable;
            set => Draggable = value;
        }

        #endregion

        #region Private Properties

        [SerializeField] private AudioSource _audioSource;

        #endregion


        #region Unity Flow



        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion

        #region IInteractable Interface Methods

        public override void StartInteraction(Transform source)
        {
            PlayerProgressService.Instance.AddBodyPart(this);
            _audioSource.Play();
            Destroy(gameObject);
        }

        public override void StopInteraction()
        {
        }

        public override string GetInteractionText()
        {
            return "Collect";
        }

        public override string GetItemName()
        {
            return Type.ToString();
        }

        #endregion
    }
}