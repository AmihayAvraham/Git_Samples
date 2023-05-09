using UnityEngine;


namespace Nokobot.Mobile.Overworked
{
    [CreateAssetMenu(fileName = "Items_Data", menuName = "Nokobot/Overworked/Items", order = 1)]
    public class ItemsAsset : ScriptableObject
    {
        public ItemStats[] Items;

        [System.Serializable]
        public struct ItemStats
        {
            public string Key;
            public ItemObject Prefab;
            public Sprite Image;
            //TODO:add action required
        }

        public ItemObject GetItemByKey(string itemKey)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Key == itemKey)
                {
                    return Items[i].Prefab;
                }
            }

            return Items[0].Prefab;
        }
        
        public ItemStats GetItemByIndex(int itemIndex)
        {
            return Items[itemIndex];
        }
    }
}
