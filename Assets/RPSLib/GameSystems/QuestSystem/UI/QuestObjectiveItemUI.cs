/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore
{
    public class QuestObjectiveItemUI : MonoBehaviour
    {
        #region Private Properties        

        #endregion

        #region Public Properties

        public string ObjectiveID;
        public TextMeshProUGUI ObjectiveText;
        public Image ObjectiveStatusImage;
        public Color InProgressColor;
        public Color CompletedColor;
        public Color FailedColor;

        public GameObject StatusIcon_Completed;
        public GameObject StatusIcon_Failed;

        #endregion


        #region Unity Flow
        #endregion        

        #region Public Methods

        public void SetData(ObjectiveSO objective)
        {
            // Set the data reference
            ObjectiveID = objective.ObjectiveID;

            // Set the objective description text
            ObjectiveText.text = objective.Title;
            if (objective.RequiredObjectiveSteps > 1)
            {
                ObjectiveText.text += $" ({objective.ObjectiveProgress}/{objective.RequiredObjectiveSteps})";
            }

            // Set status image color
            ObjectiveStatusImage.color = InProgressColor;

            // Set status icons
            StatusIcon_Completed.SetActive(false);
            StatusIcon_Failed.SetActive(false);
        }

        public void UpdateObjective(ObjectiveSO objective)
        {
            // Set status image color
            ObjectiveStatusImage.color = objective.IsComplete ? CompletedColor : InProgressColor;

            // Update visual progress            
            if (objective.RequiredObjectiveSteps > 1)
            {
                ObjectiveText.text = $"{objective.Title} ({objective.ObjectiveProgress}/{objective.RequiredObjectiveSteps})";
            }

            // Disable status icons for now
            StatusIcon_Completed.SetActive(objective.IsComplete);
            //StatusIcon_Failed.SetActive(objective.isCompleted);
        }

        #endregion

        #region Private Methods

        #endregion

    }

}