/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace GameCore
{
    public abstract class Collectable : MonoBehaviour, IInteractable
    {
        public string CollectableName;


        #region Interface Methods

        public abstract void Interact();

        #endregion
    }
}