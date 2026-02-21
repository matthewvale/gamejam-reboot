/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    TalkTo,
    GoToLocation,
    Kill,
    Collect,
    Deliver,
    Hack,
    Mine,
    Defend
}

namespace RPSCore
{
    [CreateAssetMenu(fileName = "New Objective", menuName = "Quests/New Objective")]
    public class ObjectiveSO : ScriptableObject
    {
        [HideInInspector]
        public ObjectiveData Data = new();

        [Header("Objective Details")]
        [Tooltip("Unique ID for this objective.")]
        public string ObjectiveID;
        [Tooltip("The type of objective this is, used for Event calling.")]
        public ObjectiveType ObjectiveType;
        public List<string> Parameters;

        [Tooltip("Player-facing title of this objective.")]
        public string Title;

        [Header("Progress Tracking")]
        public int RequiredObjectiveSteps = 1;
        public int ObjectiveProgress = 0;
        public bool IsComplete;


        public void AddProgress(int amount)
        {
            // Don't add progress if already complete or maximum progress reached.
            if (IsComplete || ObjectiveProgress >= RequiredObjectiveSteps)
            {
                return;
            }

            ObjectiveProgress += amount;
            Data.PROGRESS = ObjectiveProgress;

            if (ObjectiveProgress >= RequiredObjectiveSteps)
            {
                IsComplete = true;
                Data.IS_COMPLETE = IsComplete;
            }
        }
    }
}