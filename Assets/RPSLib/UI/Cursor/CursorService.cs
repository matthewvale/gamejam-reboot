/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{
    public class CursorService : MonoBehaviour
    {
        #region Instancing

        public static CursorService Instance { get; private set; }

        #endregion

        #region Public Properties

        public enum CursorType
        {
            MAIN, UI_HOVER, UI_CLICK, ATTACK, DEFEND
        }
        public Texture2D main;
        public Texture2D ui_hover;
        public Texture2D ui_click;
        public Texture2D attack;
        public Texture2D defend;

        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;

        #endregion


        #region Unity Flow


        protected void Awake()
        {
            Instance = this;

            if (main == null)
            {
                RPSLib.Debug.Log("Main cursor texture is not set in CursorService.", RPSLib.Debug.Style.Warning);
                return;
            }

            SetCursorType(CursorType.MAIN);
        }
        #endregion

        #region Public Methods

        public void SetCursorType(CursorType targetCursor)
        {
            switch (targetCursor)
            {
                case CursorType.MAIN:
                    Cursor.SetCursor(main, hotSpot, cursorMode);
                    break;
                case CursorType.UI_HOVER:
                    Cursor.SetCursor(ui_hover, hotSpot, cursorMode);
                    break;
                case CursorType.UI_CLICK:
                    Cursor.SetCursor(ui_click, hotSpot, cursorMode);
                    break;
                case CursorType.ATTACK:
                    Cursor.SetCursor(attack, hotSpot, cursorMode);
                    break;
                case CursorType.DEFEND:
                    Cursor.SetCursor(defend, hotSpot, cursorMode);
                    break;
                default:
                    Cursor.SetCursor(main, hotSpot, cursorMode);
                    break;
            }
        }

        #endregion

    }

}