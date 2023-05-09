using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    [CreateAssetMenu(menuName = "Nokobot/OverWorked/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string Key;
        public GameObject Prefab;
        public Sprite Image;
        public string CombineKey;
    }
}
