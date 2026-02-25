/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using RPSCore;
using RPSLib;
using UnityEngine;

namespace GameCore
{
    public class MainMenuController : MonoBehaviour
    {
        #region Public Properties



        #endregion

        #region Private Properties

        [SerializeField] private Canvas[] _mainMenuCanvases;


        #endregion


        #region Unity Flow



        #endregion

        #region Public Methods

        public void StartGame()
        {
            SceneManagement.LoadScene(SceneNameManager.GAME, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            RPSLib.SceneManagement.UnloadScene(SceneNameManager.MAIN_MENU);
        }

        public void ShowPanel(Canvas canvas)
        {
            for (int i = 0; i < _mainMenuCanvases.Length; i++)
            {
                _mainMenuCanvases[i].enabled = false;
            }

            canvas.enabled = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Private Methods



        #endregion

    }
}