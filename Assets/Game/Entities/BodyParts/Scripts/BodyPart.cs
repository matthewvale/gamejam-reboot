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
            Leg,
            Arm,
            Head,
            Laser
        }

        #endregion

        #region Private Properties



        #endregion


        #region Unity Flow



        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion

        #region Collectable Interface Methods

        public override void Interact()
        {
            PlayerProgressService.Instance.AddBodyPart(this);
            Destroy(gameObject);
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