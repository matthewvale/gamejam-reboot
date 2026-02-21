/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPSCore
{

    public class ContextMenuService : ManagedUI
    {

        #region Instancing

        public static ContextMenuService Instance { get; private set; }

        #endregion

        #region Private Properties

        private InputAction _RMB;
        //private InputAction _LMB;
        private InputAction _Escape;

        private List<GameObject> _contextMenuItems = new(); // Spawned button tracking

        #endregion

        #region Public Properties

        public Canvas ContextMenuCanvas;
        public RectTransform ContextMenuPanel;
        public TextMeshProUGUI ContextMenuTitleText;
        public GameObject ContextMenuItemPrefab;
        public LayerMask ContextMenuLayers;

        public bool ContextMenuOpen => ContextMenuCanvas.enabled;

        public struct ContextMenuData
        {
            public string Source;
            public List<ContextMenuItem> MenuItems;
            public bool SpawnAtMouse;
        }

        public struct ContextMenuItem
        {
            public string Text;
            public Sprite Icon;
            public Action OnOptionSelected;
            public bool Active;
        }

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            RegisterManagedUI(ContextMenuCanvas, false);

            SubscribeInputs();
        }

        private void OnDestroy()
        {
            ClearExistingContextMenu();
            UnsubscribeInputs();
        }

        private void Update()
        {
            if (!ContextMenuCanvas.enabled)
            {
                return;
            }
        }

        #endregion

        #region Private Methods

        private void SubscribeInputs()
        {
            _RMB = InputSystem.actions.FindAction(InputActionConstants.RMB);
            //_LMB = InputSystem.actions.FindAction(InputActionConstants.LMB);
            _Escape = InputSystem.actions.FindAction(InputActionConstants.Escape);

            _RMB.started += TryFindContextMenuObject;
            //_LMB.started += context => HideContextMenu();
            _Escape.started += context => HideContextMenu();
        }

        private void UnsubscribeInputs()
        {
            _RMB.started -= TryFindContextMenuObject;
            //_LMB.started -= context => HideContextMenu();
            _Escape.started -= context => HideContextMenu();
        }

        private void TryFindContextMenuObject(InputAction.CallbackContext context)
        {
            // Raycast to find object under mouse
            CameraControllerV2 camController = CameraControllerV2.Instance;
            if (camController == null)
            {
                RPSLib.Debug.Log("No camera V2 found!", RPSLib.Debug.Style.Error);
                return;
            }

            HideContextMenu(); // Clear any existing menu

            Ray ray = camController.GetCameraComponent().ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, ContextMenuLayers))
            {
                // Check if the hit object has a IContextMenuObject component
                if (hitInfo.collider.TryGetComponent(out IContextMenuObject contextMenuObject))
                {
                    SetData(contextMenuObject.GetContextMenuData());
                }
            }
        }

        private void ClearExistingContextMenu()
        {
            if (_contextMenuItems.Count == 0)
            {
                return; // Already empty and clear
            }

            // Do a reverse for loop to avoid index issues when removing items
            for (int i = _contextMenuItems.Count - 1; i >= 0; i--)
            {
                Destroy(_contextMenuItems[i]);
            }

            _contextMenuItems.Clear();
        }

        #endregion

        #region Public Methods

        public void SetData(ContextMenuData contextMenuData)
        {
            ClearExistingContextMenu();

            ContextMenuTitleText.text = contextMenuData.Source;

            for (int i = 0; i < contextMenuData.MenuItems.Count; i++)
            {
                // Add new button GameObject to cached list so we can delete it later
                GameObject newButton = Instantiate(ContextMenuItemPrefab, ContextMenuPanel);
                _contextMenuItems.Add(newButton);

                // Init this button with the data
                newButton.GetComponent<ContextMenuItemUI>().Init(contextMenuData.MenuItems[i]);
            }

            if (contextMenuData.SpawnAtMouse)
            {
                ContextMenuPanel.position = Input.mousePosition;
            }

            ShowCanvas(ContextMenuCanvas, DisplayMethod.HideAllAndShowMe, false);
        }

        public void HideContextMenu()
        {
            if (ContextMenuOpen)
            {
                //RPSLib.Debug.Log("Hiding context menu", RPSLib.Debug.Style.Success);
                ClearExistingContextMenu();
                HideCanvas(ContextMenuCanvas);
            }
        }

        #endregion

    }
}