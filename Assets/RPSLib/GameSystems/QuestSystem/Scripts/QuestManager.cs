/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPSCore
{
    public class QuestManager : ManagedUI
    {
        #region Instancing

        public static QuestManager Instance { get; private set; }

        #endregion

        #region Private Properties

        private readonly List<QuestSO> _allQuests = new();
        private readonly List<QuestSO> _possibleQuests = new();
        private readonly List<QuestSO> _activeQuests = new();
        private readonly List<QuestSO> _completedQuests = new();

        private readonly Dictionary<QuestSO, GameObject> _questUIItems = new();
        private readonly List<QuestCategoryItemUI> _questCategoryItems = new();

        #endregion

        #region Public Properties

        [Header("Quest Lists")]
        public List<QuestSO> AllKickoffQuests = new();
        public List<QuestSO> AllMainQuests = new();
        public List<QuestSO> AllTalkToQuests = new();
        public List<QuestSO> AllGoToLocationQuests = new();
        public List<QuestSO> AllKillQuests = new();
        public List<QuestSO> AllDeliverQuests = new();
        public List<QuestSO> AllHackQuests = new();
        public List<QuestSO> AllMineQuests = new();
        public List<QuestSO> AllDefendQuests = new();

        [Header("Quest Categories")]
        public List<QuestCategory> QuestCategories;
        public GameObject QuestCategoryPrefab;
        public Transform QuestCategoryParent;

        [Header("UI")]
        public Canvas QuestCanvas;
        public GameObject QuestItemPrefab;
        public Transform QuestListParent;

        public QuestItemFullUI QuestItemFullUI;

        #endregion

        #region Events/Actions

        public event Action<QuestSO> TrackedQuestChanged;
        public event Action<QuestSO> CurrentQuestUpdated;
        public event Action<QuestSO> OnQuestClaimed;

        #endregion


        #region Unity Flow

        protected void Awake()
        {
            Instance = this;

            RegisterManagedUI(QuestCanvas, false);
            SetupQuestLists();
            Init();

            //if (!SaveManager.isLoadingFromSave) {
            //    //Debug.Log("Not loading from save, Init quests...");
            //    Init();
            //}
        }

        private void OnDestroy()
        {
            _allQuests.Clear();
            _possibleQuests.Clear();
            _activeQuests.Clear();
            _completedQuests.Clear();
            _questUIItems.Clear();
            _questCategoryItems.Clear();

            Instance = null;
        }

        #endregion

        #region Private Methods        

        private void SetupQuestLists()
        {
            // Join all lists into one master list
            _allQuests.AddRange(AllMainQuests);
            _allQuests.AddRange(AllKickoffQuests);
            _allQuests.AddRange(AllTalkToQuests);
            _allQuests.AddRange(AllGoToLocationQuests);
            _allQuests.AddRange(AllKillQuests);
            _allQuests.AddRange(AllDeliverQuests);
            _allQuests.AddRange(AllHackQuests);
            _allQuests.AddRange(AllMineQuests);
            _allQuests.AddRange(AllDefendQuests);
        }

        private void Init()
        {
            Reset();

            // Kickoff quests
            for (int i = 0; i < AllKickoffQuests.Count; i++)
            {
                CreateInstanceOfQuest(AllKickoffQuests[i], true);
            }

            PopulateQuestCategories();
        }

        /// <summary>
        /// Reset the Quest Manager state, as if we're starting a new game.
        /// </summary>
        private void Reset()
        {
            _possibleQuests.Clear();
            _activeQuests.Clear();
            _completedQuests.Clear();

            foreach (var kvp in _questUIItems)
            {
                Destroy(kvp.Value);
            }
            _questUIItems.Clear();
        }

        private void PopulateQuestCategories()
        {
            foreach (GameObject existingQuestItem in _questUIItems.Values)
            {
                Destroy(existingQuestItem);
            }
            _questUIItems.Clear();

            foreach (var questCategory in QuestCategories)
            {
                QuestCategoryItemUI questCategoryItem = Instantiate(QuestCategoryPrefab, QuestCategoryParent).GetComponent<QuestCategoryItemUI>();
                questCategoryItem.SetData(questCategory.CategoryIcon, questCategory.Category.ToString(), questCategory.Category, 0);
                _questCategoryItems.Add(questCategoryItem);
            }

            ShowCategory(RPSCore.QuestCategories.ALL);
        }

        public void ShowCategory(QuestCategories category)
        {
            foreach (var kvp in _questUIItems)
            {
                Destroy(kvp.Value);
            }
            _questUIItems.Clear();

            // Update category counts
            int totalQuestCounter = 0;
            foreach (var questCategoryItem in _questCategoryItems)
            {
                int questCount = _activeQuests.Count(q => q.Category.ToString() == questCategoryItem.CategoryName.text && !q.IsClaimed);
                totalQuestCounter += questCount;
                questCategoryItem.UpdateData(questCount);
            }

            _questCategoryItems.FirstOrDefault(c => c.CategoryName.text == RPSCore.QuestCategories.ALL.ToString()).UpdateData(totalQuestCounter);

            // Populate quests for selected category
            if (category == RPSCore.QuestCategories.ALL)
            {
                foreach (var quest in _activeQuests)
                {
                    CreateQuestUIItem(quest);
                }

                foreach (var quest in _completedQuests)
                {
                    CreateQuestUIItem(quest);
                }
                SortQuestList();
                return;
            }

            var questItems = _activeQuests.Where(q => q.Category == category).ToList();
            foreach (var quest in questItems)
            {
                CreateQuestUIItem(quest);
            }
            SortQuestList();
        }

        private void CreateInstanceOfQuest(QuestSO quest, bool activateNow = false)
        {
            QuestSO questSO = CreateInstanceOfQuestAndReturn(quest);
            _possibleQuests.Add(questSO);
            if (activateNow)
            {
                AddQuestToActive(questSO);
            }
        }

        private QuestSO CreateInstanceOfQuestAndReturn(QuestSO quest)
        {
            // Create the objectives of the quest
            List<ObjectiveSO> objectives = new();
            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                // Creates a copy of the base ObjectiveSO we can manipulate without modifying the original data
                objectives.Add(Instantiate(quest.Objectives[i]));
            }

            // Creates a copy of the base QuestSO we can manipulate without modifying the original data
            QuestSO questSO = Instantiate(quest);

            // We need to manually assign the objective copies, again to aovid odifying the originals at project level
            questSO.Objectives = objectives;

            return questSO;
        }

        private void AddQuestToActive(QuestSO questToAdd)
        {
            questToAdd.Data.ID = questToAdd.QuestID;
            questToAdd.Data.IS_ACTIVE = true;
            _activeQuests.Add(questToAdd);
            CreateQuestUIItem(questToAdd);
            SetSaveableData();
        }

        private void AddProgressToObjective(int questIndex, int objectiveIndex, int amount)
        {
            _activeQuests[questIndex].Objectives[objectiveIndex].AddProgress(amount);
            CheckQuestStatus(_activeQuests[questIndex]);
        }

        private void CheckQuestStatus(QuestSO questToQuery)
        {
            bool objectivesComplete = true;
            for (int i = 0; i < questToQuery.Objectives.Count; i++)
            {
                if (!questToQuery.Objectives[i].IsComplete)
                {
                    objectivesComplete = false;
                    break;
                }
            }

            CurrentQuestUpdated?.Invoke(questToQuery);

            if (objectivesComplete)
            {
                CompleteQuest(questToQuery);
            }
        }

        private void CompleteQuest(QuestSO questToComplete)
        {
            // Update quest data...
            questToComplete.IsCompleted = true;
            questToComplete.Data.IS_COMPLETE = true;
            questToComplete.Data.IS_ACTIVE = false;

            // Update lists
            _completedQuests.Add(questToComplete);
            //_activeQuests.Remove(questToComplete);

            // UI updates        
            UpdateQuestItem(questToComplete);

            // Update save data
            SetSaveableData();

            // Check if the quest leads to another...
            if (questToComplete.TriggersNextQuest.Count > 0)
            {
                for (int i = 0; i < questToComplete.TriggersNextQuest.Count; i++)
                {
                    CreateInstanceOfQuest(questToComplete.TriggersNextQuest[i], true);
                }
            }
        }

        private void CreateQuestItem(QuestSO quest)
        {
            _questUIItems[quest].GetComponent<QuestItemUI>().InitUI(quest);
        }

        private void UpdateQuestItem(QuestSO quest)
        {
            _questUIItems[quest].GetComponent<QuestItemUI>().UpdateUI(quest);
        }

        private void DisableQuestItem(QuestSO quest)
        {
            _questUIItems[quest].GetComponent<QuestItemUI>().DisableUI();
        }

        private void SortQuestList()
        {
            _questUIItems.OrderBy(q => q.Key.IsClaimed);
        }

        private void SetSaveableData()
        {
            //GameSaveData.Instance.ActiveQuests.Clear();
            //for (int i = 0; i < activeQuests.Count; i++) {
            //    GameSaveData.Instance.ActiveQuests.Add(activeQuests[i].data);
            //}
            //GameSaveData.Instance.CompletedQuests.Clear();
            //for (int i = 0; i < completedQuests.Count; i++) {            
            //    GameSaveData.Instance.CompletedQuests.Add(completedQuests[i].data);
            //}
        }

        #endregion

        #region Public Methods

        public void OpenQuestUI()
        {
            ShowCanvas(QuestCanvas, DisplayMethod.HideAllAndShowMe, true);
        }

        public void CloseQuestUI()
        {
            HideCanvas(QuestCanvas);
        }

        public void CreateNewQuest(QuestSO newQuestSO, bool activateNow = true)
        {
            CreateInstanceOfQuest(newQuestSO, activateNow);
        }

        public void CreateQuestUIItem(QuestSO questToAdd)
        {
            // If this quest is already in the UI, ignore so we don't duplicate or spawn an empty entry
            if (_questUIItems.FirstOrDefault(q => q.Key.QuestID == questToAdd.QuestID).Key != null)
            {
                return;
            }

            // Create the pairing of the UI element and the Quest data in a dictionary, using QuestSO as key
            GameObject questItem = Instantiate(QuestItemPrefab, QuestListParent);
            if (_questUIItems.TryAdd(questToAdd, questItem))
            {
                // Update the UI with the Quest data
                CreateQuestItem(questToAdd);
            }
            else
            {
                Destroy(questItem);
            }
        }

        //public void RemoveQuestItem(QuestSO quest)
        //{
        //    // Delete UI object
        //    Destroy(_questUIItems[quest]);
        //    // Remove Quest from Dictionary
        //    _questUIItems.Remove(quest);
        //}

        public void SelectQuest(QuestSO quest)
        {
            QuestItemFullUI.SetData(quest);
        }

        public void TrackQuest(QuestSO quest)
        {
            TrackedQuestChanged?.Invoke(quest);
        }

        //public void AddProgressToQuest(QuestSO questToProgress, int amount)
        //{
        //    var quest = _possibleQuests.FirstOrDefault(q => q.QuestID == questToProgress.QuestID);
        //    if (quest != null)
        //    {
        //        for (int i = 0; i < questToProgress.Objectives.Count; i++)
        //        {
        //            ObjectiveSO objective = questToProgress.Objectives[i];
        //            if (objective.ObjectiveProgress < objective.RequiredObjectiveSteps)
        //            {
        //                objective.ObjectiveProgress++;
        //                if (objective.ObjectiveProgress >= objective.RequiredObjectiveSteps)
        //                {
        //                    objective.IsComplete = true;
        //                }
        //                objective.Data.PROGRESS = objective.ObjectiveProgress;
        //                objective.Data.IS_COMPLETE = objective.IsComplete;
        //                CheckQuestStatus(questToProgress);
        //                //SetSaveableData();
        //                break; // Only add progress to one objective at a time
        //            }
        //        }
        //    }
        //}

        public void ClaimRewards(QuestSO questData)
        {
            // Normal reward claim
            if (questData.Rewards is { Count: > 0 })
            {
                for (int i = 0; i < questData.Rewards.Count; i++)
                {
                    questData.Rewards[i].Reward();
                }
            }

            // Bonus reward claim
            if (questData.BonusRewardsEarned)
            {
                if (questData.BonusRewards is { Count: > 0 })
                {
                    for (int i = 0; i < questData.BonusRewards.Count; i++)
                    {
                        questData.BonusRewards[i].Reward();
                    }
                }
            }

            // Remove claimed quest from on-screen tracker
            //RemoveQuestItem(questData);
            DisableQuestItem(questData);
            OnQuestClaimed?.Invoke(questData);

            // Remove Quest Full Info from UI           
            QuestItemFullUI.Hide();

            // Remove from active quests so we don't show it, and add to completed
            _activeQuests.Remove(questData);
            _completedQuests.Add(questData);

            // Mark as claimed
            questData.IsClaimed = true;
            questData.Data.IS_CLAIMED = true;
        }

        public void ActivateQuestOfTypeToBeCompleted(string ID, bool allowDuplicate = false)
        {
            if (allowDuplicate == false)
            {
                if (HasQuestCompleted(ID) || IsQuestActive(ID))
                    return;
            }
            CreateInstanceOfQuest(GetPossibleQuestByID(ID), true);
        }

        public QuestSO GetPossibleQuestByID(string ID)
        {
            for (int i = 0; i < _possibleQuests.Count; i++)
            {
                if (_possibleQuests[i].QuestID == ID)
                    return _possibleQuests[i];
            }
            return null;
        }

        public QuestSO GetActiveQuestByID(string ID)
        {
            if (_activeQuests.Count == 0)
                return null;

            for (int i = 0; i < _activeQuests.Count; i++)
            {
                if (_activeQuests[i].QuestID == ID)
                {
                    return _activeQuests[i];
                }
            }

            return null;
        }

        public bool HasQuestCompleted(string ID)
        {
            if (_completedQuests.Count == 0)
                return false;
            for (int i = 0; i < _completedQuests.Count; i++)
            {
                if (_completedQuests[i].QuestID == ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsQuestActive(string ID)
        {
            if (_activeQuests.Count == 0)
                return false;

            for (int i = 0; i < _activeQuests.Count; i++)
            {
                if (_activeQuests[i].QuestID == ID)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadActiveQuests(List<QuestData> data)
        {
            // Loop through all possible quests, if we get a match of ID, recreate it and set to active
            for (int i = 0; i < _allQuests.Count; i++)
            {
                //Debug.Log("Possible quest 1 query...");
                for (int n = 0; n < data.Count; n++)
                {
                    if (_allQuests[i].QuestID == data[n].ID)
                    {
                        //Debug.Log("Loading Active Quest :: " + questList[i].QUEST_ID);
                        CreateInstanceOfQuest(_allQuests[i], true);
                    }
                }
            }
        }

        public void LoadCompletedQuests(List<QuestData> data)
        {
            for (int i = 0; i < _allQuests.Count; i++)
            {
                for (int n = 0; n < data.Count; n++)
                {
                    //Debug.Log("LoadCompletedQuests :: Data :: " + data[n].ID);
                    if (_allQuests[i].QuestID == data[n].ID)
                    {
                        _completedQuests.Add(_allQuests[i]);
                    }
                }
            }
        }

        #endregion

        #region Objective events

        public void TriggerEvent(ObjectiveType objectiveType, int amount, object context = null)
        {
            if (_activeQuests.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _activeQuests.Count; i++)
            {
                for (int o = _activeQuests[i].Objectives.Count - 1; o >= 0; o--)
                {
                    if (_activeQuests[i].Objectives[o].ObjectiveType != objectiveType)
                    {
                        continue;
                    }

                    switch (context)
                    {
                        case null:
                            // Null is a valid context, just add progress
                            AddProgressToObjective(i, o, amount);
                            break;

                        case string strContext:
                            if (_activeQuests[i].Objectives[o].Parameters.Count > 0)
                            {
                                if (!string.Equals(_activeQuests[i].Objectives[o].Parameters[0], strContext, StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }
                                AddProgressToObjective(i, o, amount);
                            }
                            break;

                        case ItemSO itemContext:
                            if (_activeQuests[i].Objectives[o].Parameters.Count > 0)
                            {
                                if (!string.Equals(_activeQuests[i].Objectives[o].Parameters[0], itemContext.ItemID, StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }
                                AddProgressToObjective(i, o, amount);
                            }
                            break;

                        default:
                            RPSLib.Debug.Log($"Unhandled context type: {context.GetType()}", RPSLib.Debug.Style.CriticalError, true);
                            return;
                    }
                }
            }
        }

        #endregion

    }

}