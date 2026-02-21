/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;

namespace RPSCore
{
    public class TransactionInventory : IInventory
    {
        #region Private Properties

        private readonly BaseInventory _source;

        private Dictionary<ItemSO, int> _items;
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

        #endregion


        public TransactionInventory(BaseInventory source)
        {
            _source = source;
            MaxInventorySlots = _source.MaxInventorySlots;
            OccupiedInventorySlots = _source.OccupiedInventorySlots;
            _items = new Dictionary<ItemSO, int>(source.Items);
            _currencies = new Dictionary<CurrencySO, int>(source.Currencies);
        }

        public void PairInventoryView(InventoryView inventoryView)
        {
            InventoryView = inventoryView;
        }

        public bool AddItem(ItemSO item, TransactionType transaction, int amount)
        {
            // If adding to the player's transaction inventory, attempt to apply currency first for buys
            bool isPlayerSource = _source == PlayerInventoryService.Instance.PlayerInventory;

            if (transaction == TransactionType.ADD)
            {
                // If this is the player and they're buying, subtract currency first (fail if unaffordable)
                if (isPlayerSource)
                {
                    int cost = item.ItemValue * amount;
                    if (!AddCurrency(CurrencyHandler.Instance.GalacticCredits, TransactionType.SUBTRACT, cost))
                    {
                        // Can't afford, abort the add
                        return false;
                    }
                }

                if (!_items.ContainsKey(item))
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

            // If subtracting from the player's transaction inventory (player sold items), give credits
            if (transaction == TransactionType.SUBTRACT && isPlayerSource)
            {
                int payout = item.ItemValue * amount;
                // AddCurrency will update UI via InventoryView if present
                AddCurrency(CurrencyHandler.Instance.GalacticCredits, TransactionType.ADD, payout);
            }

            if (InventoryView != null)
            {
                InventoryView.AddItem(item, transaction, amount);
            }

            return true;
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

        public void Commit()
        {
            _source.SetState(_items, _currencies);
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

        public int GetItemAmount(ItemSO item)
        {
            if (_items.ContainsKey(item))
            {
                return _items[item];
            }

            return 0;
        }
    }

}