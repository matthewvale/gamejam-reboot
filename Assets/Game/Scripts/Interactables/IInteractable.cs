
using UnityEngine;

/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------
namespace GameCore
{
    public interface IInteractable
    {
        public abstract bool IsSingleUse { get; set; }
        public abstract bool IsDraggable { get; set; }

        public abstract void StartInteraction(Transform source);

        public abstract void StopInteraction();

        public abstract string GetInteractionText();

        public abstract string GetItemName();
    }
}