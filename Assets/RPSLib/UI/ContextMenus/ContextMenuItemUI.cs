/* Author:		Matthew Vale
 * Role:		Lead Game Developer
 * Company:		Red Phoenix Studios
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore {

    public class ContextMenuItemUI : MonoBehaviour {

        public Image Image;
        public TextMeshProUGUI Text;
        public Button Button;

        public void Init(ContextMenuService.ContextMenuItem itemData) {
            Image.gameObject.SetActive(itemData.Icon);
            Image.sprite = itemData.Icon;
            Text.text = itemData.Text;

            Button.onClick.AddListener(() => {
                itemData.OnOptionSelected?.Invoke();
                ContextMenuService.Instance.HideContextMenu();
            });

            Button.interactable = itemData.Active;
        }

    }

}