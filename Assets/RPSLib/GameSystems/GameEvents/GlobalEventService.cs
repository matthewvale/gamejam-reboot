/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RPSCore
{
    public class GlobalEventService : MonoBehaviour
    {
        #region Instancing

        public static GlobalEventService Instance { get; private set; }

        #endregion

        #region Private Properties

        private ActivityFeedService ActivityFeedService => _activityFeedService != null ? _activityFeedService : _activityFeedService = ActivityFeedService.Instance;
        private ActivityFeedService _activityFeedService;

        private List<GlobalEventEntry> _permanentEventEntries = new();

        #endregion

        #region Public Properties

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            CacheData();
        }

        #endregion

        #region Private Methods

        private void CacheData()
        {

        }

        #endregion

        #region Public Methods

        public void AddNewEntry(EventConstants.EventType type, string entryText, bool permanent, bool postToActivityfeed = false, Sprite entrySprite = null)
        {
            GlobalEventEntry newEntry = new(type, entryText);

            if (permanent)
            {
                _permanentEventEntries.Add(newEntry);
            }

            if (postToActivityfeed)
            {
                ActivityFeedService.AddNewEntry(entrySprite, Color.white, entryText);
            }
        }

        #endregion

    }

}