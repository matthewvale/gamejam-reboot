/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine.EventSystems;

namespace RPSCore
{

    public class InventorySlot : ButtonInteractor
    {

        #region Private Properties

        private ItemUIItem _item;
        private InventoryView _inventoryView;

        #endregion


        #region Public Methods

        public void Initialize(InventoryView inventoryView)
        {
            _inventoryView = inventoryView;
        }

        public void SetItem(ItemUIItem item)
        {
            _item = item;

            // Must be placing, not removing, so set inventory slot
            if (item != null)
            {
                _item.SetInventorySlot(this);
            }
        }

        public bool IsOccupied()
        {
            return _item != null;
        }

        // ----- Pointer Events ----- //
        // -------------------------- //
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            // Ensure left click only
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // If we have an item already selected...
                if (InventoryInteractionService.Instance.HasItemSelected)
                {
                    // If this is an empty slot, tell inventory to move item here...
                    if (!IsOccupied())
                    {
                        ItemUIItem selectedItem = InventoryInteractionService.Instance.SelectedItem;
                        if (_inventoryView == selectedItem.GetInventoryView())
                        {
                            // Moving within the same inventory, so just move it.
                            _inventoryView.MoveItemToSlot(selectedItem, this);
                        }
                        else
                        {
                            // Moving between different inventories, so remove from old and add to new.
                            int amount = selectedItem.Amount;
                            selectedItem.GetInventoryView().TransferItem(selectedItem.Item, TransactionType.SUBTRACT, selectedItem.Amount);
                            _inventoryView.TransferItem(selectedItem.Item, TransactionType.ADD, amount);
                            _inventoryView.MoveItemToSlot(selectedItem, this);
                        }
                    }
                }
            }
        }

        #endregion

    }

}