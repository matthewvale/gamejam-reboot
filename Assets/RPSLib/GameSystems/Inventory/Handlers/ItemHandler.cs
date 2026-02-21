/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPSCore
{

    public class ItemHandler : MonoBehaviour
    {
        #region Instancing

        public static ItemHandler Instance { get; private set; }

        #endregion

        #region Private Properties

        // Cached Dictionary lookup for quick access to an item by ID - O(1) complexity
        private Dictionary<string, ItemSO> _itemsByIDLookup = new();
        private Dictionary<ItemType, List<ItemSO>> _itemsByTypeLookup = new();

        #endregion

        #region Public Propertiies

        public List<ItemList> AllItemLists = new();

        [System.Serializable]
        public struct ItemList
        {
            public RarityLevel Rarity;
            public List<ItemSO> Items;
        }

        #endregion

        #region Unity Flow

        protected void Awake()
        {
            Instance = this;

            BuildItemsByIDLookup();
            BuildItemsByTypeLookup();
        }

        #endregion

        #region Private Methods

        private void BuildItemsByIDLookup()
        {
            _itemsByIDLookup = AllItemLists
                .SelectMany(itemList => itemList.Items)
                .ToDictionary(item => item.ItemID, item => item);
        }

        private void BuildItemsByTypeLookup()
        {
            for (int i = 0; i < AllItemLists.Count; i++)
            {
                for (int j = 0; j < AllItemLists[i].Items.Count; j++)
                {
                    if (!_itemsByTypeLookup.TryGetValue(AllItemLists[i].Items[j].ItemType, out List<ItemSO> items))
                    {
                        items = new List<ItemSO>();
                        _itemsByTypeLookup[AllItemLists[i].Items[j].ItemType] = items;
                    }

                    items.Add(AllItemLists[i].Items[j]);
                }
            }
        }

        #endregion

        #region Public Methods

        public ItemSO GetRandomItemSO(RarityLevel? rarityTarget = null)
        {
            if (rarityTarget == null)
            {
                int rList = Random.Range(0, AllItemLists.Count);
                int rItem = Random.Range(0, AllItemLists[rList].Items.Count);
                return AllItemLists[rList].Items[rItem];
            }

            ItemList filteredItemList = AllItemLists.FirstOrDefault(x => x.Rarity == rarityTarget);
            if (filteredItemList.Items == null || filteredItemList.Items.Count == 0)
            {
                RPSLib.Debug.Log($"No items found for rarity: {rarityTarget}!", RPSLib.Debug.Style.CriticalError);
                return null;
            }

            return filteredItemList.Items[Random.Range(0, filteredItemList.Items.Count)];
        }

        public ItemSO GetItemSOByID(string ID)
        {
            ItemSO itemSo = _itemsByIDLookup.TryGetValue(ID, out ItemSO item) ? item : null;

            if (itemSo == null)
            {
                RPSLib.Debug.Log($"Item with ID: {ID} not found!", RPSLib.Debug.Style.CriticalError);
            }

            return itemSo;
        }

        public ItemSO GetItemByItemType(ItemType type)
        {
            if (_itemsByTypeLookup.TryGetValue(type, out List<ItemSO> items) && items.Count > 0)
            {
                return items[Random.Range(0, items.Count)];
            }

            RPSLib.Debug.Log($"No items found for type: {type}!", RPSLib.Debug.Style.CriticalError);
            return null;
        }

        #endregion
    }

}