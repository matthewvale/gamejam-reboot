/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore
{
    public class GlobalManagersScene : MonoBehaviour
    {

        private void Start()
        {
            RPSLib.SceneManagement.LoadScene(SceneNameManager.RPS_INTRO, LoadSceneMode.Additive, true);
        }

    }
}