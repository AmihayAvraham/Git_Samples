using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public static class ItemManager
    {
        private static ItemData[] _items;
        private static ItemObject _itemObjectBase;
        public static ItemData[] Items
        {
            get
            {
                if(_items == null)
                {
                    _items = Resources.LoadAll<ItemData>("Items");
                }
                return _items;
            }
        }

        public static ItemObject ItemObjectBase
        {
            get
            {
                if (_itemObjectBase == null)
                {
                    _itemObjectBase = Resources.Load<ItemObject>("Items/ItemObjectBase");
                }
                return _itemObjectBase;
            }
        }
        
        public static ItemData GetItemByKey(string itemKey)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Key == itemKey)
                {
                    return Items[i];
                }
            }

            return Items[0];
        }
        
        public static ItemData GetItemByIndex(int itemIndex)
        {
            return Items[itemIndex];
        }

        public static ItemObject GetNewItemObject() => ItemObjectBase;
    }
}
