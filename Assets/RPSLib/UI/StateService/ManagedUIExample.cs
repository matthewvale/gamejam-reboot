
using UnityEngine;

/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------
namespace RPSCore
{
    public class ManagedUIExample : ManagedUI
    {
        public Canvas ExampleCanvas;

        void Awake()
        {
            RegisterManagedUI(ExampleCanvas, true);
        }

        public void ShowExampleCanvas()
        {
            ShowCanvas(ExampleCanvas, DisplayMethod.HideAllAndShowMe, true);
        }

        public void HideExampleCanvas()
        {
            HideCanvas(ExampleCanvas);
        }

    }
}