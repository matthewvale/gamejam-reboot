/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore
{

    public class QuestCategoryItemUI : MonoBehaviour
    {

        public Image CategoryImage;
        public TextMeshProUGUI CategoryName;
        public TextMeshProUGUI CategoryQuestCount;
        public Color QuestCountTextColor;
        public Color EmptyQuestCountTextColor;
        public Button CategoryButton;

        public void SetData(Sprite icon, string name, QuestCategories category, int questCount)
        {
            CategoryImage.sprite = icon;
            CategoryName.text = name;
            UpdateData(questCount);

            CategoryButton.onClick.AddListener(() =>
            {
                QuestManager.Instance.ShowCategory(category);
            });
        }

        public void UpdateData(int questCount)
        {
            CategoryQuestCount.text = $"{questCount}";
            if (string.Equals(CategoryQuestCount.text, "0"))
            {
                CategoryButton.interactable = false;
                CategoryQuestCount.color = EmptyQuestCountTextColor;
            }
            else
            {
                CategoryButton.interactable = true;
                CategoryQuestCount.color = QuestCountTextColor;
            }
        }

    }

}