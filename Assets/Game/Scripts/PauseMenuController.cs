/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using JetBrains.Annotations;
using RPSCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore
{
    public class PauseMenuController : MonoBehaviour
    {
        #region Public Properties



        #endregion

        #region Private Properties

        private InputAction _escapeAction;

        [SerializeField] private Canvas _pauseCanvas;
        private bool _isLoading = false;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            _escapeAction = InputSystem.actions.FindAction(InputActionConstants.Escape);
            _escapeAction.started -= ctx => TogglePauseMenu();
            _escapeAction.started += ctx => TogglePauseMenu();

            _pauseCanvas.enabled = false;
            _isLoading = false;
        }

        private void OnDestroy()
        {
            _escapeAction.started -= ctx => TogglePauseMenu();
        }

        #endregion

        #region Public Methods

        [UsedImplicitly]
        public void LoadMainMenu()
        {
            if (_isLoading)
            {
                return;
            }
            _isLoading = true;
            TogglePauseMenu();
            CursorService.Instance?.SetCursorType(CursorService.CursorType.MAIN, CursorLockMode.None);
            PlayerProgressService.Instance.ExitToMenu();
        }

        public void TogglePauseMenu()
        {
            if (!_pauseCanvas.enabled)
            {
                _pauseCanvas.enabled = true;
                CursorService.Instance.SetCursorType(CursorService.CursorType.MAIN, CursorLockMode.None);
                Time.timeScale = 0f;
            }
            else
            {
                _pauseCanvas.enabled = false;
                CursorService.Instance.SetCursorType(CursorService.CursorType.MAIN, CursorLockMode.Locked);
                Time.timeScale = 1f;
            }
        }

        #endregion

        #region Private Methods



        #endregion

    }
}