/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RPSCore
{
    public abstract class ManagedUI : MonoBehaviour
    {
        [Header("ManagedUI Valid Scenes")]
        public List<string> ValidScenes;
        public bool PausesGame = false;
        public bool PreventsPlayerInput = false;

        public virtual void RegisterManagedUI(Canvas canvas, bool initialState)
        {
            UIStateService.Instance.RegisterManagedUI(canvas, this, initialState);
        }

        public virtual void ShowCanvas(Canvas canvas, DisplayMethod displayMethod, bool addToStack)
        {
            UIStateService.Instance.ShowCanvas(canvas, displayMethod, addToStack, ValidScenes);
        }

        public virtual void HideCanvas(Canvas canvas)
        {
            UIStateService.Instance.HideCanvas(canvas);
        }

        public virtual void OnCanvasShow() { }

        public virtual void OnCanvasHide() { }

    }
}