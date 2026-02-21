/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System;
using UnityEngine;

namespace RPSCore
{

    public class PlayerInventoryService : ManagedUI
    {

        #region Instancing

        public static PlayerInventoryService Instance { get; private set; }

        #endregion

        #region Private Properties     

        private const string _playeInventoryName = "Personal Cache";

        #endregion

        #region Public Propertiies

        [Header("Player Inventory Canvas")]
        public Canvas PlayerInventoryCanvas;
        public BaseInventory PlayerInventory; // The player's main inventory.
        public InventoryView PlayerInventoryView;
        public bool PlayerInventoryOpen => PlayerInventoryCanvas.enabled;
        public int MaxInventorySlots = 64;

        public int StartingGalacticCredits { get; private set; } = 500000;

        #endregion

        #region Events

        public event Action<CurrencySO, int> OnCurrencyUpdated;

        #endregion


        #region Unity Flow

        private void Awake()
        {
            Instance = this;
            RegisterManagedUI(PlayerInventoryCanvas, false);
        }

        protected void Start()
        {
            PlayerInventoryCanvas.enabled = false;
            PlayerInventory.MaxInventorySlots = MaxInventorySlots;
            PlayerInventoryView.SetData(PlayerInventory, _playeInventoryName);

            GiveStartingResources();

            PlayerInventory.OnCurrencyUpdated += UpdateCurrencyAmount;
        }

        private void OnDisable()
        {
            PlayerInventory.OnCurrencyUpdated -= UpdateCurrencyAmount;
        }

        #endregion

        #region Private Methods

        private void SendToActivityFeed(ItemSO item, int amount, TransactionType transaction)
        {
            string addedremoved = transaction == TransactionType.ADD ? "<color=green>Added</color>" : "<color=red>Removed</color>";
            string tofrom = transaction == TransactionType.ADD ? "to" : "from";

            ActivityFeedService.Instance.AddNewEntry(
                item.ItemIcon,
                item.GetRarityPrimaryColor(),
                $"{addedremoved} {amount} <color=#{ColorUtility.ToHtmlStringRGB(item.GetRarityPrimaryColor())}>{(string.IsNullOrEmpty(item.ItemName) ? "ITEM_NAME_MISSING" : item.ItemName)}</color> {tofrom} inventory."
            );
        }

        private void SendToActivityFeed(CurrencySO currency, int amount, TransactionType transaction)
        {
            string addedremoved = transaction == TransactionType.ADD ? "<color=green>Added</color>" : "<color=red>Removed</color>";
            string tofrom = transaction == TransactionType.ADD ? "to" : "from";

            ActivityFeedService.Instance.AddNewEntry(
                currency.CurrencyIcon,
                currency.CurrencyColor,
                $"{addedremoved} {amount} <color=#{ColorUtility.ToHtmlStringRGB(currency.CurrencyColor)}>{(string.IsNullOrEmpty(currency.ItemName) ? "ITEM_NAME_MISSING" : currency.ItemName)}</color> {tofrom} inventory."
            );
        }

        private void UpdateCurrencyAmount(CurrencySO currency, int newAmount)
        {
            OnCurrencyUpdated?.Invoke(currency, newAmount);
        }

        #endregion

        #region Public Methods

        public void AddCurrency(CurrencySO currency, TransactionType transaction, int amount, bool silent = false)
        {
            if (PlayerInventory.AddCurrency(currency, transaction, amount))
            {                                
                if (!silent)
                {
                    SendToActivityFeed(currency, amount, transaction);
                }
            }
        }

        public void AddItem(ItemSO item, TransactionType transaction, int amount, bool silent = false)
        {
            if (PlayerInventory.AddItem(item, transaction, amount))
            {
                QuestManager.Instance.TriggerEvent(ObjectiveType.Collect, amount, item);

                if (!silent)
                {
                    SendToActivityFeed(item, amount, transaction);
                }
            }
        }

        public void CloseInventory()
        {
            InventoryInteractionService.Instance.ClosePlayerInventory();
        }

        // TODO First mission in the game should call this instead.
        public void GiveStartingResources()
        {
            AddCurrency(CurrencyHandler.Instance.GalacticCredits, TransactionType.ADD, StartingGalacticCredits, true);
            AddCurrency(CurrencyHandler.Instance.BitShards, TransactionType.ADD, 512, true);
            AddCurrency(CurrencyHandler.Instance.Quantumite, TransactionType.ADD, 5, true);
            AddItem(ItemHandler.Instance.GetItemSOByID("fuel"), TransactionType.ADD, 5000, true);
        }

        #endregion

    }

}