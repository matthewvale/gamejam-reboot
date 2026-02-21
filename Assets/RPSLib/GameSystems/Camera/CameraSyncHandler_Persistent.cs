/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public static class CameraSyncHandler_Persistent
    {

        private static Vector3 _storedEulerAngles = Vector3.zero;

        // TODO :: Could we just use an event here instead?
        public static bool FetchExpected = false;


        public static void StoreCameraRotation(Vector3 rot, bool nextEnabledCameraShouldFetch)
        {
            _storedEulerAngles = rot;
            FetchExpected = nextEnabledCameraShouldFetch;
        }

        public static Vector3 FetchCameraRotation(bool shouldClear = true)
        {
            FetchExpected = false;

            if (shouldClear)
            {
                Vector3 returnVal = _storedEulerAngles;
                _storedEulerAngles = default;
                return returnVal;
            }

            return _storedEulerAngles;
        }

    }

}