/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace RPSCore
{

    public class InventoryInteractionService : MonoBehaviour
    {

        #region Instancing

        public static InventoryInteractionService Instance { get; private set; }

        #endregion

        #region Private Properties

        private InputAction _mousePosition;
        private InputAction _rightMouseButton;
        private InputAction _cancelButton;

        private Dictionary<GameObject, IInventory> _openInventories = new(); // Multiple inventories can be open at once.

        private PlayerInventoryService _playerInventoryService => PlayerInventoryService.Instance;

        #endregion

        #region Public Properties

        [Header("Root Inventory Canvas")]
        public Canvas RootInventoryCanvas;

        [Header("Spawnable Inventory View")]
        public GameObject InstancedInventoryViewPrefab; // The base prefab for any inventory view.

        // Any item being moved stored here.
        [HideInInspector] public ItemUIItem SelectedItem;
        public bool HasItemSelected => SelectedItem != null;
        [HideInInspector] public InventorySlot SelectedItemInventorySlot;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            SubscribeInput();
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
        }

        void Update()
        {
            // TODO replace with InputSystem
            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    if (!_playerInventoryService.PlayerInventoryOpen)
            //    {
            //        OpenPlayerInventory();
            //    }
            //    else
            //    {
            //        ClosePlayerInventory();
            //    }
            //}

            if (Input.GetKeyDown(KeyCode.R))
            {
                TEST_GiveRandomItems(1, 1);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TEST_GiveSpecificItem("fuel", 500);
            }

            if (SelectedItem != null)
            {
                SelectedItem.transform.position = _mousePosition.ReadValue<Vector2>();
            }
        }

        #endregion

        #region Private Methods

        private void SubscribeInput()
        {
            _mousePosition = InputSystem.actions.FindAction(InputActionConstants.MousePosition);

            _rightMouseButton = InputSystem.actions.FindAction(InputActionConstants.RMB);
            _rightMouseButton.started += CancelSelectedItem;

            _cancelButton = InputSystem.actions.FindAction(InputActionConstants.Escape);
            _cancelButton.started += CancelSelectedItem;
        }

        private void UnsubscribeInput()
        {
            _rightMouseButton.started -= CancelSelectedItem;
            _cancelButton.started -= CancelSelectedItem;
        }

        #endregion

        #region Public Methods        

        public void OpenPlayerInventory()
        {
            _playerInventoryService.PlayerInventoryCanvas.enabled = true;
            _openInventories.Add(_playerInventoryService.PlayerInventoryCanvas.gameObject, _playerInventoryService.PlayerInventory);
        }

        public void ClosePlayerInventory()
        {
            _playerInventoryService.PlayerInventoryCanvas.enabled = false;
            _openInventories.Remove(_playerInventoryService.PlayerInventoryCanvas.gameObject);

            if (SelectedItem != null)
            {
                SelectedItem.ReturnToOriginalInventoryPosition();
            }
        }

        public void OpenInventory(BaseInventory inventory, string sourceText, bool playerOwned)
        {
            if (_openInventories.ContainsValue(inventory))
            {
                RPSLib.Debug.Log("Failed to open inventory, it may already be open.", RPSLib.Debug.Style.Warning);
                return;
            }

            GameObject inventoryViewGO = Instantiate(InstancedInventoryViewPrefab, RootInventoryCanvas.transform);
            inventoryViewGO.GetComponent<InventoryView>().SetData(inventory, sourceText);

            if (!_openInventories.TryAdd(inventoryViewGO, inventory))
            {
                // Failsafe - should never hit this.
                Destroy(inventoryViewGO);
                RPSLib.Debug.Log("Failed to open inventory, it may already be open.", RPSLib.Debug.Style.Warning);
            }
        }

        public void CloseInventory(IInventory inventory)
        {
            foreach (var kvp in _openInventories)
            {
                if (kvp.Value == inventory)
                {
                    Destroy(kvp.Key);
                    _openInventories.Remove(kvp.Key);
                    break;
                }
            }

            if (SelectedItem != null)
            {
                SelectedItem.ReturnToOriginalInventoryPosition();
            }
        }

        public void CloseAllInventories()
        {
            foreach (var kvp in _openInventories)
            {
                Destroy(kvp.Key);
            }
            _openInventories.Clear();
        }

        public void CancelSelectedItem(CallbackContext ctx)
        {
            if (SelectedItem != null)
            {
                SelectedItem.ReturnToOriginalInventoryPosition();
            }
        }

        #endregion

        #region TESTS

        private void TEST_GiveRandomItems(int numberOfItems, int amountPerItem)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                _playerInventoryService.AddItem(ItemHandler.Instance.GetRandomItemSO(), TransactionType.ADD, amountPerItem);
            }
        }

        private void TEST_GiveSpecificItem(string id, int amountPerItem)
        {
            _playerInventoryService.AddItem(ItemHandler.Instance.GetItemSOByID(id), TransactionType.ADD, amountPerItem);
        }

        private void TEST_RemoveFirstStack()
        {
            _playerInventoryService.AddCurrency(CurrencyHandler.Instance.GalacticCredits, TransactionType.SUBTRACT, 6666);
            _playerInventoryService.AddItem(ItemHandler.Instance.GetRandomItemSO(), TransactionType.SUBTRACT, 100);
        }

        #endregion

    }
}