/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore
{
    public class QuestItemUI : MonoBehaviour
    {
        private QuestSO _questData;

        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public Image QuestCategoryImage;
        public Image QuestCategoryBackground;
        public Image QuestGradientImage;
        public Color ClaimedColor;
        public Color ClaimedTextColor;

        [Header("BUTTONS")]
        public Button QuestItemButton;
        public GameObject claimButton;
        public GameObject trackButton;


        public void InitUI(QuestSO data)
        {
            // Show basic quest data
            _questData = data;
            Title.text = _questData.Title;
            Description.text = _questData.Description;
            QuestCategoryImage.sprite = QuestManager.Instance.QuestCategories.Find(x => x.Category == _questData.Category).CategoryIcon;
            QuestCategoryBackground.sprite = QuestCategoryImage.sprite;

            if (data.IsClaimed)
            {
                DisableUI();
                return;
            }

            // Update certain UI elements
            UpdateUI(_questData);
        }

        public void UpdateUI(QuestSO data)
        {
            // Show basic quest data
            _questData = data;

            // Quest complete state
            claimButton.SetActive(_questData.IsCompleted);
            trackButton.SetActive(!_questData.IsCompleted);
        }

        public void DisableUI()
        {
            QuestItemButton.interactable = false;
            claimButton.SetActive(false);
            trackButton.SetActive(false);
            QuestGradientImage.color = ClaimedColor;
            Title.color = ClaimedTextColor;
            Description.color = ClaimedTextColor;
        }

        public void ClaimReward()
        {
            QuestManager.Instance.ClaimRewards(_questData);
        }

        public void SelectQuestItem()
        {
            QuestManager.Instance.SelectQuest(_questData);
        }

        public void TrackQuest()
        {
            QuestManager.Instance.TrackQuest(_questData);
        }
    }

}