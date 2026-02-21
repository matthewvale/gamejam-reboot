/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPSCore
{

    public class ItemUIItem : ButtonInteractor
    {

        #region Private Properties
        private RectTransform _transform;
        private Button _button;
        private CanvasGroup _canvasGroup;

        private InventoryView _inventoryView;
        private InventorySlot _inventorySlot;
        #endregion

        #region Public Properties       
        public Image ItemImage;
        public Button HightlightButton;
        public Image RarityPrimaryImage;
        public Image RarityBorderImage;
        public Image RarityBorderGlowImage;
        public Image RaritySecondaryImage;
        public GameObject IllegalImage;
        public Image AmountContainerImage;
        public TextMeshProUGUI AmountText;

        public ItemSO Item { get; set; }
        public int Amount { get; set; }
        #endregion


        #region Public Methods
        public void SetData(ItemSO itemSO, int amount, InventoryView inventoryView)
        {
            _transform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _inventoryView = inventoryView;
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryView.UIItemSize);

            Item = itemSO;

            ItemImage.sprite = Item.ItemIcon;

            ColorBlock colors = HightlightButton.colors;
            Color highlightedColor = Item.GetRarityColorBorder();
            highlightedColor.a = .75f;
            colors.highlightedColor = highlightedColor;
            HightlightButton.colors = colors;

            RarityPrimaryImage.color = Item.GetRarityPrimaryColor();
            RarityBorderImage.color = Item.GetRarityColorBorder();
            RarityBorderGlowImage.color = Item.GetRarityColorBorder();
            RaritySecondaryImage.color = Item.GetRaritySecondaryColor();
            Color amountContainerColor = Item.GetRarityColorBorder();
            amountContainerColor.a = .25f;
            AmountContainerImage.color = amountContainerColor;

            IllegalImage.SetActive(Item.Illegal);
            if (Item.Illegal)
            {
                RarityBorderImage.color = ItemRarityHandler.Instance.RarityColors.IllegalBorder;
                AmountContainerImage.color = ItemRarityHandler.Instance.RarityColors.IllegalBorder;
            }

            UpdateValue(amount, false);
        }

        public void UpdateValue(int amount, bool destroyIfZero)
        {
            Amount = amount;
            AmountText.text = $"{Amount:n0}"; // 1,234,456 - No decimals

            if (destroyIfZero && Amount <= 0)
            {
                _inventorySlot.SetItem(null);
                Destroy(gameObject);
            }
        }

        public RectTransform GetRectTransform()
        {
            return _transform;
        }

        public void SetInventorySlot(InventorySlot slot)
        {
            _inventorySlot = slot;
        }

        public InventorySlot GetInventorySlot()
        {
            return _inventorySlot;
        }

        public void DisableItemButton()
        {
            _button.interactable = false;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void EnableItemButton()
        {
            _button.interactable = true;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void ReturnToOriginalInventoryPosition()
        {
            _inventoryView.MoveItemToSlot(this, _inventorySlot);
        }

        public void RemoveFromInventory()
        {
            _inventoryView.AddItem(Item, TransactionType.SUBTRACT, Amount);
        }

        public InventoryView GetInventoryView()
        {
            return _inventoryView;
        }

        // ----- Pointer Events ----- //
        // -------------------------- //
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            // Ensure left click only
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (InventoryInteractionService.Instance.HasItemSelected)
                {
                    //RPSLib.Debug.Log($"Slot is occupied by {Item.ItemID}, can't add {InventoryInteractionService.Instance.SelectedItem.Item.ItemID}", RPSLib.Debug.Style.Informational);
                    return;
                }
                // Select this slot item if we don't already have one selected...
                if (!InventoryInteractionService.Instance.HasItemSelected)
                {
                    _inventoryView.SelectItem(this);
                }
            }
        }

        #endregion

    }

}