/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPSCore
{
    public class RPSIntro : MonoBehaviour
    {
        private string gameName;
        private string year;
        private const float _loadMainMenuAfter = 12f;

        public TextMeshProUGUI copyrightText;


        private void Start()
        {
            gameName = GameVersion.GameName;
            year = DateTime.Now.Year.ToString();

            copyrightText.text =
                "Copyright Red Phoenix Studios "
                + year
                + ". Red Phoenix Studios and "
                + gameName
                + " are protected under copyright law.";
            
            Invoke(nameof(LoadMainMenu), _loadMainMenuAfter);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                LoadMainMenu();
            }
        }

        private void LoadMainMenu()
        {
            CancelInvoke();
            RPSLib.SceneManagement.LoadScene(SceneNameManager.MAIN_MENU, LoadSceneMode.Additive, true);
            RPSLib.SceneManagement.UnloadScene(SceneNameManager.RPS_INTRO);
        }

    }

}