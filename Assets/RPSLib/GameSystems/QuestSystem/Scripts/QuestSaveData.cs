/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

namespace RPSCore
{
    [System.Serializable]
    public class QuestData
    {
        public string ID;
        public bool IS_ACTIVE;
        public bool IS_COMPLETE;
        public bool IS_CLAIMED;
    }

    [System.Serializable]
    public class ObjectiveData
    {
        public string ID;
        public int PROGRESS;
        public bool IS_COMPLETE;
    }

}