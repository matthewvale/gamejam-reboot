/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public class TriggerZoneHandler : MonoBehaviour, IIsTrigger
    {

        #region Private Properties

        private IIsTrigger _trigger;

        #endregion

        #region Public Properties

        public GameObject ObjectToTrigger;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            CacheData();
        }

        #endregion

        #region Private Methods

        private void CacheData()
        {
            _trigger = ObjectToTrigger.GetComponent<IIsTrigger>();
        }

        #endregion

        #region Public Methods


        #endregion

        #region Interface Implementation

        public bool IsActive { get; set; }

        public void ActivateTrigger()
        {
            _trigger.ActivateTrigger();
        }

        public void DeactivateTrigger()
        {
            _trigger.DeactivateTrigger();
        }

        #endregion

    }
}