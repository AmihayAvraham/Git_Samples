using UnityEngine;

namespace RopeMan.Store
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoreItem", order = 1)]
    public class StoreItem : ScriptableObject
    {
        public string itemName;

        public string ID;
        public Sprite icon;
        public int price;
    }
}