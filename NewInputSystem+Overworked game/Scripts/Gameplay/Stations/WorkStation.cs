using System;
using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public abstract class WorkStation : MonoBehaviour
    {
        public Transform _snapTransform;
        public ItemObject _snappedItem;
        public bool _hasItem;

        public abstract void PutDown(ItemObject item);
        public abstract ItemObject Pickup();
        public abstract void DoAction();
        public abstract bool HasItem();
        public abstract void CombineItems(ItemObject item);
    }
}
