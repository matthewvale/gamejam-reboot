/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RPSCore
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
    public class QuestSO : ScriptableObject
    {
        [HideInInspector]
        public QuestData Data = new();

        [Header("Quest Details")]
        [Tooltip("Unique ID for this quest.")]
        public string QuestID;

        [Tooltip("Will this quest always be active in the background?")]
        public bool IsPassive;

        [Tooltip("Can this quest be repeated after completion?")]
        public bool IsRepeatable;

        [Tooltip("Category this quest belongs to. For filtering.")]
        public QuestCategories Category;

        [Tooltip("Who or what gave this quest?")]
        public string SourceOfQuest;

        [Tooltip("Player-facing title of this quest.")]
        public string Title;

        [Tooltip("Player-facing description of this quest")]
        [TextArea] public string Description;

        [Header("Objectives")]
        public List<ObjectiveSO> Objectives;

        [Header("Quest State")]
        public bool IsActive;
        public bool IsCompleted;
        public bool IsClaimed;

        [Header("Rewards")]
        public List<QuestReward> Rewards;
        public List<QuestReward> BonusRewards; // Could be used for completing a task perfectly
        [HideInInspector] public bool BonusRewardsEarned;

        [Header("Prerequisites")]
        [Tooltip("Which quests should we have completed before this one is active?")]
        public List<QuestSO> MustHaveCompleted;

        [Header("Triggers")]
        [Tooltip("Which quests should we trigger after this one is completed?")]
        public List<QuestSO> TriggersNextQuest;
    }

}