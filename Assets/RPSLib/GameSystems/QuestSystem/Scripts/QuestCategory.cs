/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{
    [System.Serializable]
    public class QuestCategory
    {
        public QuestCategories Category;
        public Sprite CategoryIcon;
    }

    public enum QuestCategories
    {
        ALL,
        MAIN,
        CORPO,
        BOUNTY,
        RESCUE,
        DELIVERY,
        EXPLORATION,
        MINING,
        ESCORT,
        HACKING
    }

}