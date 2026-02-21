/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using Steamworks;
using UnityEngine;

namespace RPSCore.Steamworks
{
    public class PlatformDependantSDKManager : MonoBehaviour
    {

        #region Instancing

        public static PlatformDependantSDKManager Instance { get; private set; }

        #endregion

        #region Public Properties

        public Platform platform;
        public enum Platform
        {
            Standalone,
            Steam
        }

        #endregion


        #region Unity Flow

        protected void Start()
        {
            Instance = this;

            if (platform != Platform.Steam)
            {
                SteamAPI.Shutdown();
            }
        }

        #endregion

    }

}