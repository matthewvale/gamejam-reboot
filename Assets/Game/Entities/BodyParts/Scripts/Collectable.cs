/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace GameCore
{
    public abstract class Collectable : MonoBehaviour, IInteractable
    {
        public string CollectableName;

        public abstract bool IsSingleUse { get; set; }
        public abstract bool IsDraggable { get; set; }


        #region Interface Methods

        public abstract void StartInteraction(Transform source);

        public abstract void StopInteraction();

        public abstract string GetInteractionText();

        public abstract string GetItemName();

        #endregion
    }
}