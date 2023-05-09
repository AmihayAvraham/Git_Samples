using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public class ItemObject : MonoBehaviour
    {
        private ItemData _itemData;

        public void Init(string itemKey)
        {
            _itemData = ItemManager.GetItemByKey(itemKey);
            Instantiate(_itemData.Prefab, transform);
        }

        public string GetCurrentKey() => _itemData.Key;
        public string GetCombineKey() => _itemData.CombineKey;
    }
}
