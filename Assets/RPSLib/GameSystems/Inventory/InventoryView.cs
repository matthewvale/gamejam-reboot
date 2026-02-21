/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPSCore
{

    public class InventoryView : MonoBehaviour
    {

        #region Private Properties

        private InventoryInteractionService _inventoryInteractionService;

        [Header("Inventory Grid Sizes")]
        public float UIItemSize = 32f;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;

        private IInventory _inventory;
        private List<InventorySlot> _inventorySlots = new();
        private List<ItemUIItem> _itemUIItems = new();
        private List<CurrencyUIItem> _currencyUIItems = new();

        private bool _isMovingWindowView = false;
        private Vector3 _dragOffset;

        #endregion

        #region Public Properties

        public Canvas RootCanvas;

        [Header("Inventory Slot Base")]
        public GameObject InventorySlotPrefab;

        [Header("Currencies UI")]
        public GameObject CurrencyUIItemPrefab;
        public RectTransform CurrencyPanelRoot;

        [Header("Items UI")]
        public GameObject ItemUIItemPrefab;
        public RectTransform ItemPanelRoot;

        public bool IsOpen => gameObject.activeSelf;

        public RectTransform WindowViewPanel;

        public TextMeshProUGUI InventorySourceText;

        #endregion

        #region Actions/Evens

        public event Action<Dictionary<CurrencySO, int>> OnInventoryUpdated;

        #endregion


        #region Unity Flow

        public void SetData(IInventory inventory, string sourceText)
        {
            ResetView();

            _inventoryInteractionService = InventoryInteractionService.Instance;

            _inventory = inventory;
            _inventory.PairInventoryView(this);

            InventorySourceText.SetText($"{sourceText}'s inventory");

            _gridLayoutGroup.cellSize = new Vector2(UIItemSize, UIItemSize);
            SpawnInventorySlots(_inventory.MaxInventorySlots);
            SpawnItems();
        }

        private void Update()
        {
            if (_isMovingWindowView)
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                WindowViewPanel.position = mousePos + _dragOffset;
            }
        }

        #endregion

        #region Private Methods

        private void ResetView()
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                Destroy(_inventorySlots[i].gameObject);
            }
            _inventorySlots.Clear();

            for (int i = 0; i < _itemUIItems.Count; i++)
            {
                Destroy(_itemUIItems[i].gameObject);
            }
            _itemUIItems.Clear();
        }

        private void SpawnInventorySlots(int maxSlots)
        {
            for (int i = 0; i < maxSlots; i++)
            {
                InventorySlot newSlot = Instantiate(InventorySlotPrefab, ItemPanelRoot).GetComponent<InventorySlot>();
                newSlot.Initialize(this);
                _inventorySlots.Add(newSlot);
            }
        }

        private void SpawnItems()
        {
            foreach (var item in _inventory.Items)
            {
                AddToStack(item.Key, TransactionType.ADD, item.Value);
            }

            SortInventoryUIItems(ResourceSortingType.RARITY);
        }

        // Item Stacks
        // ------------------------------

        private bool AddToStack(ItemSO item, TransactionType transaction, int amount)
        {
            if (transaction == TransactionType.ADD)
            {
                // Adding to existing stacks
                int remainingAmount = amount;

                for (int i = 0; i < _itemUIItems.Count; i++)
                {
                    if (_itemUIItems[i].Item == item)
                    {
                        // If we find a full stack of this item, go to next
                        if (_itemUIItems[i].Amount >= item.MaxStack)
                        {
                            continue;
                        }

                        // If we find a stack of this item, update the amount and check for overflow
                        int currentAmount = _itemUIItems[i].Amount;
                        int spaceLeft = item.MaxStack - currentAmount;
                        int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);

                        _itemUIItems[i].UpdateValue(_itemUIItems[i].Amount + amountToAdd, false);
                        remainingAmount -= amountToAdd;

                        if (remainingAmount <= 0)
                        {
                            break;
                        }
                    }
                }

                // Check if we are in overflow, if we are, make new stacks to accommodate the remaining amount
                while (remainingAmount > 0)
                {
                    if (_inventory.InventoryFull)
                    {
                        RPSLib.Debug.Log("Inventory is full, cannot add more items.", RPSLib.Debug.Style.Informational);
                        return false;
                    }

                    int overflowAmount = Mathf.Min(item.MaxStack, remainingAmount);
                    // Make new stack
                    ItemUIItem newItem = CreateItemStack(item, overflowAmount);
                    AddStackToFirstEmptySlot(newItem);
                    remainingAmount -= overflowAmount;
                }
            }

            if (transaction == TransactionType.SUBTRACT)
            {
                int remainingAmount = amount;

                for (int i = _itemUIItems.Count - 1; i >= 0; i--)
                {
                    if (_itemUIItems[i].Item == item)
                    {
                        int currentAmount = _itemUIItems[i].Amount;
                        int amountToRemove = Mathf.Min(currentAmount, remainingAmount);
                        int newAmount = currentAmount - amountToRemove;

                        _itemUIItems[i].UpdateValue(newAmount, true);
                        if (newAmount <= 0)
                        {
                            _itemUIItems.RemoveAt(i);
                        }

                        remainingAmount -= amountToRemove;

                        if (remainingAmount <= 0)
                        {
                            break;
                        }
                    }
                }
            }

            return true;
        }

        private ItemUIItem CreateItemStack(ItemSO item, int amount)
        {
            // Create it...
            ItemUIItem newItem = Instantiate(ItemUIItemPrefab).GetComponent<ItemUIItem>();
            // Set it's initial Data...
            newItem.SetData(item, amount, this);
            // Add to the list of existing UI items...
            _itemUIItems.Add(newItem);

            return newItem;
        }

        private void AddStackToFirstEmptySlot(ItemUIItem newItem)
        {
            // Add to first empty inventory slot
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (!_inventorySlots[i].IsOccupied())
                {
                    PlaceItemInInventorySlot(newItem, _inventorySlots[i]);
                    break;
                }
            }
        }

        private void PlaceItemInInventorySlot(ItemUIItem item, InventorySlot inventorySlot)
        {
            inventorySlot.SetItem(item);

            // Make sure our item matches our inventory slot sizes, this is dynamic.
            item.transform.SetParent(inventorySlot.transform);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(UIItemSize, UIItemSize);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            item.EnableItemButton();
        }

        // Currency UI Updates
        // ------------------------------

        private void UpdateCurrencyUI()
        {
            // Spawn currency items if they don't exist yet
            foreach (var currency in _inventory.Currencies)
            {
                var existingCurrencyItem = _currencyUIItems.Find(c => c.Currency == currency.Key);
                if (existingCurrencyItem != null)
                {
                    existingCurrencyItem.UpdateValue(currency.Value);
                    continue;
                }

                CurrencyUIItem newCurrencyItem = Instantiate(CurrencyUIItemPrefab, CurrencyPanelRoot).GetComponent<CurrencyUIItem>();
                newCurrencyItem.SetData(currency.Key, currency.Value);
                _currencyUIItems.Add(newCurrencyItem);
            }

            SortCurrencyUIItems(ResourceSortingType.RARITY);
        }

        // Sorting Methods
        // ------------------------------

        private void SortCurrencyUIItems(ResourceSortingType sortingMethod)
        {
            switch (sortingMethod)
            {
                case ResourceSortingType.NONE:
                    _currencyUIItems.Sort((x, y) => x.Currency.ItemRarity.CompareTo(y.Currency.ItemRarity));
                    break;
                case ResourceSortingType.QUANTITY:
                    _currencyUIItems.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                    break;
                case ResourceSortingType.RARITY:
                    _currencyUIItems.Sort((x, y) => x.Currency.ItemRarity.CompareTo(y.Currency.ItemRarity));
                    break;
                case ResourceSortingType.ALPHABETICALLY:
                    _currencyUIItems.Sort((x, y) => x.Currency.ItemID.CompareTo(y.Currency.ItemID));
                    break;
                default:
                    _currencyUIItems.Sort((x, y) => x.Currency.ItemRarity.CompareTo(y.Currency.ItemRarity));
                    break;
            }

            UpdateCurrencySortOrder();
        }

        private void SortInventoryUIItems(ResourceSortingType sortingMethod)
        {
            switch (sortingMethod)
            {
                case ResourceSortingType.NONE:
                    _itemUIItems.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                    break;
                case ResourceSortingType.QUANTITY:
                    _itemUIItems.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                    break;
                case ResourceSortingType.RARITY:
                    _itemUIItems.Sort((x, y) => x.Item.ItemRarity.CompareTo(y.Item.ItemRarity));
                    break;
                case ResourceSortingType.ALPHABETICALLY:
                    _itemUIItems.Sort((x, y) => x.Item.ItemID.CompareTo(y.Item.ItemID));
                    break;
                case ResourceSortingType.PRICE_PER_UNIT:
                    _itemUIItems.Sort((x, y) => x.Item.ItemValue.CompareTo(y.Item.ItemValue));
                    break;
                default:
                    _itemUIItems.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                    break;
            }

            UpdateInventorySortOrder();
        }

        private void UpdateCurrencySortOrder()
        {
            for (int i = 0; i < _currencyUIItems.Count; i++)
            {
                _currencyUIItems[i].transform.SetSiblingIndex(i);
            }
        }

        private void UpdateInventorySortOrder()
        {
            // Clear all inventory slots...
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].SetItem(null);
            }

            // Re-place all items in order...
            for (int i = 0; i < _itemUIItems.Count; i++)
            {
                PlaceItemInInventorySlot(_itemUIItems[i], _inventorySlots[i]);
            }
        }

        #endregion

        #region Button Handlers

        [UsedImplicitly]
        public void CloseInventory()
        {
            _inventoryInteractionService.CloseInventory(_inventory);
        }

        [UsedImplicitly]
        public void StartMovingWindowView()
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            _dragOffset = WindowViewPanel.position - mousePos;
            _isMovingWindowView = true;
        }

        [UsedImplicitly]
        public void StopMovingWindowView()
        {
            _isMovingWindowView = false;
        }

        [UsedImplicitly]
        public void SortByRarity()
        {
            SortInventoryUIItems(ResourceSortingType.RARITY);
        }

        [UsedImplicitly]
        public void SortByBasePrice()
        {
            SortInventoryUIItems(ResourceSortingType.PRICE_PER_UNIT);
        }

        [UsedImplicitly]
        public void SortByName()
        {
            SortInventoryUIItems(ResourceSortingType.ALPHABETICALLY);
        }

        [UsedImplicitly]
        public void SortByQuantity()
        {
            SortInventoryUIItems(ResourceSortingType.QUANTITY);
        }

        #endregion

        #region Public Methods

        public void UpdateCurrencies()
        {
            UpdateCurrencyUI();
            OnInventoryUpdated?.Invoke(_inventory.Currencies);
        }

        public void AddItem(ItemSO item, TransactionType transactionType, int amount)
        {
            AddToStack(item, transactionType, amount);
        }

        public void SelectItem(ItemUIItem selectedItem)
        {
            _inventoryInteractionService.SelectedItem = selectedItem;
            _inventoryInteractionService.SelectedItem.DisableItemButton();

            // Get the inventory slot this item was in and empty it...
            _inventoryInteractionService.SelectedItemInventorySlot = selectedItem.GetInventorySlot();
            _inventoryInteractionService.SelectedItemInventorySlot.SetItem(null);

            // Set the new parent of the item to the inventory canvas...
            // ... this let's us see the item and drag it around the canvas
            selectedItem.GetRectTransform().SetParent(RootCanvas.transform);
        }

        public bool HasItemSelected(out ItemUIItem selectedItem)
        {
            if (_inventoryInteractionService.SelectedItem != null)
            {
                selectedItem = _inventoryInteractionService.SelectedItem;
                return true;
            }
            else
            {
                selectedItem = null;
                return false;
            }
        }

        public bool HasItemSelected()
        {
            return _inventoryInteractionService.SelectedItem != null;
        }

        public void MoveItemToSlot(ItemUIItem item, InventorySlot slot)
        {
            if (slot.IsOccupied())
            {
                AddStackToFirstEmptySlot(item);
            }
            else
            {
                PlaceItemInInventorySlot(item, slot);
            }

            _inventoryInteractionService.SelectedItem = null;
            _inventoryInteractionService.SelectedItemInventorySlot = null;
        }

        public void TransferItem(ItemSO item, TransactionType transactionType, int amount)
        {
            _inventory.AddItem(item, transactionType, amount);
            OnInventoryUpdated?.Invoke(_inventory.Currencies);
        }

        #endregion

    }
}