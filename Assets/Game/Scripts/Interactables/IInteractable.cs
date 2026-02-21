/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

namespace GameCore
{
    public interface IInteractable
    {
        public abstract void Interact();

        public abstract string GetInteractionText();

        public abstract string GetItemName();
    }
}