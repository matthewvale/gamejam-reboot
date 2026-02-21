/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPSCore
{
    public enum DisplayMethod
    {
        HideAllAndShowMe,
        ShowMeAboveExisting
    }

    [DefaultExecutionOrder(-1)] // Make sure this runs before any other UI-related scripts
    public class UIStateService : MonoBehaviour
    {
        #region Instancing

        public static UIStateService Instance { get; private set; }

        #endregion

        #region Private Properties

        private Dictionary<Canvas, ManagedUI> _canvasDictionary = new();
        private Stack<Canvas> _canvasStack = new();

        private Canvas _currentCanvas;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        if (_canvasStack.Count > 1)
        //        {
        //            HideCanvas(_currentCanvas);
        //        }
        //    }
        //}

        #endregion

        #region Private Methods

        private void HideAll()
        {
            // Do a reverse for loop to avoid index issues when removing items
            for (int i = _canvasDictionary.Keys.Count - 1; i >= 0; i--)
            {
                if (_canvasDictionary.Keys.ElementAt(i) == null)
                {
                    //RPSLib.Debug.Log($"Canvas {_canvasDictionary.ElementAt(i)} no longer available. Removing.", RPSLib.Debug.Style.Informational);
                    _canvasDictionary.Remove(_canvasDictionary.Keys.ElementAt(i));
                    continue;
                }

                _canvasDictionary[_canvasDictionary.Keys.ElementAt(i)].OnCanvasHide();
                _canvasDictionary.Keys.ElementAt(i).enabled = false;
            }
        }

        private void TryShowPrevious()
        {
            // If the stack is not empty, pop the last canvas and show it
            // If there is only 1 item in the stack, don't close it
            if (_canvasStack.Count > 1)
            {
                _canvasStack.Pop();
                Canvas previousCanvas = _canvasStack.First();
                ShowCanvas(previousCanvas, DisplayMethod.ShowMeAboveExisting, true);
            }
        }

        private void ShowTargetCanvas(Canvas canvas, bool addToStack)
        {
            if (_canvasDictionary.ContainsKey(canvas))
            {
                bool canvasAlreadyShowing = canvas.enabled;

                // Enable the canvas
                canvas.enabled = true;
                _canvasDictionary[canvas].OnCanvasShow();

                // Add to history stack if we need to
                if (addToStack)
                {
                    if (!_canvasStack.Contains(canvas))
                    {
                        _canvasStack.Push(canvas);
                    }
                }

                // Run animation if it has one
                if (!canvasAlreadyShowing && canvas.TryGetComponent(out UIAnimator animator))
                {
                    animator.Play();
                }

                // Set the current canvas to the one being shown
                _currentCanvas = canvas;

                //RPSLib.Debug.Log($"Showing canvas {_canvasDictionary[canvas].gameObject.name}. AddToStack = {addToStack}", RPSLib.Debug.Style.Success);
            }
            else
            {
                RPSLib.Debug.Log($"Canvas not found in dictionary.", RPSLib.Debug.Style.Error);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a Canvas with its associated ManagedUI.
        /// </summary>
        public void RegisterManagedUI(Canvas canvas, ManagedUI managedUI, bool initialState)
        {
            if (_canvasDictionary.TryAdd(canvas, managedUI))
            {
                canvas.enabled = initialState;
                //RPSLib.Debug.Log($"Registered {canvas.gameObject.name} for {managedUI.GetType().Name}.", RPSLib.Debug.Style.Normal);
            }
            else
            {
                RPSLib.Debug.Log($"Canvas {canvas.gameObject.name} already registered for {managedUI.GetType().Name}.", RPSLib.Debug.Style.Warning);
            }
        }

        /// <summary>
        /// Show a specific Canvas based on the DisplayMethod.
        /// </summary>
        public void ShowCanvas(Canvas UI, DisplayMethod displayMethod, bool addToStack = false, List<string> validScenes = null)
        {
            if (validScenes != null && validScenes?.Count > 0)
            {
                if (!validScenes.Contains(SceneManager.GetActiveScene().name))
                {
                    RPSLib.Debug.Log($"Current scene does not support displaying the {UI.gameObject.name} canvas.", RPSLib.Debug.Style.Informational);
                    return;
                }
            }
            switch (displayMethod)
            {
                case DisplayMethod.HideAllAndShowMe:
                    HideAll();
                    break;
                case DisplayMethod.ShowMeAboveExisting:
                    break;
                default:
                    RPSLib.Debug.Log($"Unknown display method: {displayMethod}.", RPSLib.Debug.Style.CriticalError);
                    break;
            }

            if (UI != null)
            {
                ShowTargetCanvas(UI, addToStack);
            }
        }

        /// <summary>
        /// Hide a specific Canvas.
        /// </summary>
        public void HideCanvas(Canvas UI)
        {
            // Try to show the canvas of the specified type
            if (_canvasDictionary.ContainsKey(UI))
            {
                UI.enabled = false;
                _canvasDictionary[UI].OnCanvasHide();
                TryShowPrevious();
                //RPSLib.Debug.Log($"Hiding {_canvasDictionary[UI].gameObject.name}.", RPSLib.Debug.Style.Informational);
            }
            else
            {
                RPSLib.Debug.Log($"Canvas {UI.gameObject.name} not found in dictionary.", RPSLib.Debug.Style.Error);
            }
        }

        /// <summary>
        /// Hide all registered canvases.
        /// </summary>
        public void HideAllCanvases()
        {
            HideAll();
        }

        public bool IsCanvasStackEmpty()
        {
            return _canvasStack.Count == 0;
        }

        #endregion

    }
}