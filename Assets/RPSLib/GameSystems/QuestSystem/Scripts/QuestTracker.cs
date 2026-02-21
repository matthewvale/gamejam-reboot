/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore
{
    public class QuestTracker : MonoBehaviour
    {
        #region Private Properties

        private QuestSO _questData;
        private List<GameObject> _objectiveContainer = new();

        #endregion

        #region Public Properties

        public Canvas QuestTrackerCanvas;
        public RectTransform QuestContainerRectTransform;
        public RectTransform ObjectiveContainerParent;        
        public TextMeshProUGUI TrackedQuestTitle;
        public GameObject TrackedQuestObjectiveContainer;

        #endregion


        #region Unity Flow

        protected void Awake()
        {
            QuestTrackerCanvas.enabled = false;
        }

        protected void OnEnable()
        {
            SubscribeListeners();
        }

        protected void OnDisable()
        {
            UnsubscribeListeners();
        }

        #endregion

        #region Private Methods

        private void SubscribeListeners()
        {
            QuestManager.Instance.TrackedQuestChanged += UpdateTrackedQuest;
            QuestManager.Instance.CurrentQuestUpdated += UpdateCurrentQuest;
            QuestManager.Instance.OnQuestClaimed += QuestClaimed;
        }

        private void UnsubscribeListeners()
        {
            QuestManager.Instance.TrackedQuestChanged -= UpdateTrackedQuest;
            QuestManager.Instance.CurrentQuestUpdated -= UpdateCurrentQuest;
            QuestManager.Instance.OnQuestClaimed -= QuestClaimed;
        }

        private void RemoveTrackedQuest()
        {
            if (_objectiveContainer.Count > 0)
            {
                foreach (var objective in _objectiveContainer)
                {
                    Destroy(objective);
                }
                _objectiveContainer.Clear();
            }

            TrackedQuestTitle.text = string.Empty;

            QuestTrackerCanvas.enabled = false;            
        }

        private void UpdateTrackedQuest(QuestSO questToTrack)
        {
            // If we select the same quest to track, untrack/hide it
            if (_questData != null)
            {
                if (questToTrack.QuestID == _questData.QuestID)
                {
                    _questData = null;
                    RemoveTrackedQuest();
                    return;
                }
            }

            _questData = questToTrack;

            // Delete any existing objective data objects
            RemoveTrackedQuest();

            // Set tracked quest title
            TrackedQuestTitle.text = questToTrack.Title;

            // Spawn all objectives
            if (questToTrack.Objectives is { Count: > 0 })
            {
                foreach (var objective in questToTrack.Objectives)
                {
                    GameObject newObjectiveContainer = Instantiate(TrackedQuestObjectiveContainer, ObjectiveContainerParent);
                    newObjectiveContainer.GetComponent<QuestObjectiveItemUI>().SetData(objective);
                    _objectiveContainer.Add(newObjectiveContainer);
                }
            }

            QuestTrackerCanvas.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(QuestContainerRectTransform);
        }

        private void UpdateCurrentQuest(QuestSO updatedQuest)
        {            
            // If we have no currently tracked quest, don't try to update anything
            if (_questData == null)
            {
                return;
            }

            // If the updated quest is not the tracked quest, do nothing
            if (updatedQuest.QuestID != _questData.QuestID)
            {
                return;
            }

            // Update each objective UI item
            foreach (var objectiveUI in _objectiveContainer)
            {
                var objectiveItemUI = objectiveUI.GetComponent<QuestObjectiveItemUI>();
                var updatedObjective = updatedQuest.Objectives.Find(o => o.ObjectiveID == objectiveItemUI.ObjectiveID);
                if (updatedObjective != null)
                {
                    objectiveItemUI.UpdateObjective(updatedObjective);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(QuestContainerRectTransform);
        }

        private void QuestClaimed(QuestSO claimedQuest)
        {
            // No currently tracked quest on screen, don't try removing it
            if (_questData == null)
            {
                return;
            }

            // If the claimed quest is the tracked quest, remove it from the tracker
            if (claimedQuest.QuestID == _questData.QuestID)
            {
                _questData = null;
                RemoveTrackedQuest();
            }
        }

        #endregion

        #region Public Methods

        #endregion

    }

}