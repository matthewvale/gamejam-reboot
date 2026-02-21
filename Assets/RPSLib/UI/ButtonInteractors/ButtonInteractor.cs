/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPSCore
{
    public class ButtonInteractor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Button _btn;


        #region Unity Flow

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        #endregion

        #region Private Methods

        private bool ButtonIsValid()
        {
            if (AudioManager.Instance == null)
            {
                RPSLib.Debug.Log("ButtonInteractor :: No AudioManager found, is it in the scene?", RPSLib.Debug.Style.Warning);
            }

            if (_btn != null && !_btn.interactable)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Interface Implementation Methods

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!ButtonIsValid())
            {
                return;
            }

            CursorService.Instance.SetCursorType(CursorService.CursorType.UI_HOVER);
            AudioManager.Instance.PlayUISound(AudioManager.UIClipType.BUTTON_HOVER, true);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!ButtonIsValid())
                {
                    return;
                }

                AudioManager.Instance.PlayUISound(AudioManager.UIClipType.BUTTON_CLICK, true);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            CursorService.Instance.SetCursorType(CursorService.CursorType.MAIN);
        }

        #endregion

    }

}