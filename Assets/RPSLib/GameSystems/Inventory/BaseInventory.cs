/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPSCore
{

    public class BaseInventory : MonoBehaviour, IInventory
    {

        #region Private Properties

        private Dictionary<ItemSO, int> _items = new();
        private Dictionary<CurrencySO, int> _currencies = new();

        #endregion

        #region Public Properties

        public int MaxInventorySlots { get; set; }
        public int OccupiedInventorySlots { get; set; }
        public bool InventoryFull => OccupiedInventorySlots >= MaxInventorySlots;        

        public InventoryView InventoryView { private get; set; }

        public string SourceText { get; private set; }

        public Dictionary<ItemSO, int> Items => _items;
        public Dictionary<CurrencySO, int> Currencies => _currencies;

        public event Action<CurrencySO, int> OnCurrencyUpdated;

        #endregion


        #region Private Methods
        #endregion

        #region Public Methods

        public void PairInventoryView(InventoryView inventoryView)
        {
            InventoryView = inventoryView;
        }

        public bool AddCurrency(CurrencySO currency, TransactionType transaction, int amount)
        {
            // If we don't already have this type of item stored...
            if (!_currencies.ContainsKey(currency))
            {
                // Add to the private currency dictionary.
                _currencies.Add(currency, 0);
            }

            // Perform transaction
            if (transaction == TransactionType.ADD)
            {
                _currencies[currency] += amount;
            }
            else
            {
                // If the amount would be less than 0 and we can't go into debt, ignore
                if (_currencies[currency] - amount < 0 && !currency.CanBeInDebt)
                {
                    return false;
                }

                // Otherwise, subtract this transaction
                _currencies[currency] -= amount;
            }

            if (InventoryView != null)
            {
                InventoryView.UpdateCurrencies();
            }

            return true;
        }

        public bool AddItem(ItemSO item, TransactionType transaction, int amount)
        {
            if (transaction == TransactionType.ADD)
            {
                if (!_items.ContainsKey(item) && !InventoryFull)
                {
                    // Add to the private item dictionary.
                    _items.Add(item, 0);
                }
            }

            if (transaction == TransactionType.SUBTRACT)
            {
                if (!CanAfford(item, amount))
                {
                    return false;
                }
            }

            // Perform transaction - data only
            if (transaction == TransactionType.ADD)
            {
                _items[item] += amount;
            }
            else
            {
                _items[item] -= amount;
            }

            // Remove from the list - data only, which we can always rely on to be correct
            if (_items[item] <= 0)
            {
                _items.Remove(item);
            }

            if (InventoryView != null)
            {
                InventoryView.AddItem(item, transaction, amount);
            }

            return true;
        }

        public bool CanAfford(Currency targetCurrency, int amount)
        {
            foreach (var currency in _currencies)
            {
                if (currency.Key.Currency == targetCurrency)
                {
                    return currency.Value >= amount;
                }
            }

            return false;
        }

        public bool CanAfford(ItemSO targetItem, int amount)
        {
            foreach (var item in _items)
            {
                if (item.Key == targetItem)
                {
                    return item.Value >= amount;
                }
            }

            return false;
        }

        public int GetCurrencyAmount(CurrencySO currency)
        {
            if (_currencies.ContainsKey(currency))
            {
                return _currencies[currency];
            }

            return 0;
        }

        public int GetItemAmount(ItemSO item)
        {
            if (_items.ContainsKey(item))
            {
                return _items[item];
            }

            return 0;
        }

        public void SetState(Dictionary<ItemSO, int> items, Dictionary<CurrencySO, int> currencies)
        {
            _items = items;
            _currencies = currencies;
            if (InventoryView != null)
            {
                InventoryView.SetData(this, SourceText);
                InventoryView.UpdateCurrencies();                
            }

            OnCurrencyUpdated?.Invoke(null, GetCurrencyAmount(CurrencyHandler.Instance.GalacticCredits));
        }

        #endregion

    }
}