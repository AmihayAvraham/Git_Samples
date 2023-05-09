using System;
using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public class WorkStation_Hammer : WorkStation
    {
        [SerializeField] private String _acceptingItemKey;
        [SerializeField] private String _producingItemKey;
        public override bool HasItem() => _hasItem;
        public override void PutDown(ItemObject item)
        {
            if (_hasItem)
            {
                return;
            }
            _hasItem = true;
            _snappedItem = item;
            Transform snappedItemTransform;
            (snappedItemTransform = _snappedItem.transform).SetParent(_snapTransform);
            snappedItemTransform.localPosition = Vector3.zero;
            snappedItemTransform.localRotation = Quaternion.identity;
        }

        public override ItemObject Pickup()
        {
            if (!_hasItem)
            {
                return null;
            }
            _hasItem = false;
            ItemObject itemToReturn = _snappedItem;
            _snappedItem = null;
            return itemToReturn;
        }

        public override void DoAction()
        {
            if (_snappedItem == null)
            {
                return;
            }
            if (_snappedItem.GetCurrentKey() != _acceptingItemKey)
            {
                return;
            }
            _snappedItem.gameObject.SetActive(false);
            _snappedItem = null;
            _snappedItem = Instantiate(ItemManager.GetNewItemObject(),_snapTransform);
            var snappedItemTransform = _snappedItem.transform;
            snappedItemTransform.localPosition = Vector3.zero;
            snappedItemTransform.localRotation = Quaternion.identity;
            _snappedItem.Init(_producingItemKey);
        }
        public override void CombineItems(ItemObject item)
        {
            _snappedItem.gameObject.SetActive(false);
            _snappedItem = null;
            _snappedItem = Instantiate(ItemManager.GetNewItemObject(),_snapTransform);
            var snappedItemTransform = _snappedItem.transform;
            snappedItemTransform.localPosition = Vector3.zero;
            snappedItemTransform.localRotation = Quaternion.identity;
            _snappedItem.Init(item.GetCombineKey());
        }
    }
}
