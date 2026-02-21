/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;

namespace RPSCore
{
    public interface IInventory
    {
        public int MaxInventorySlots { get; set; }
        public int OccupiedInventorySlots { get; set; }
        public bool InventoryFull => OccupiedInventorySlots >= MaxInventorySlots;

        Dictionary<ItemSO, int> Items { get; }
        Dictionary<CurrencySO, int> Currencies { get; }

        bool AddItem(ItemSO item, TransactionType transaction, int amount);        

        bool AddCurrency(CurrencySO currency, TransactionType transaction, int amount);

        bool CanAfford(ItemSO targetItem, int amount);

        int GetItemAmount(ItemSO item);

        void PairInventoryView(InventoryView inventoryView);
    }
}