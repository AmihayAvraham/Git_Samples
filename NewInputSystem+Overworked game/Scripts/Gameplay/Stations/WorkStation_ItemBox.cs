using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nokobot.Mobile.Overworked
{
    
    public class WorkStation_ItemBox : WorkStation
    {
        [SerializeField] private Image _boxItemImage;
        [SerializeField] private String _producingItemKey;

        private void Awake()
        {
            _boxItemImage.sprite = ItemManager.GetItemByKey(_producingItemKey).Image;
        }

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
                ItemObject obejctToReturn = Instantiate(ItemManager.GetNewItemObject(),transform);
                obejctToReturn.Init(_producingItemKey);
                return obejctToReturn;
            }
            _hasItem = false;
            ItemObject itemToReturn = _snappedItem;
            _snappedItem = null;
            return itemToReturn;
        }

        public override void DoAction()
        {
            /*ItemObject obejctToReturn = Instantiate(ItemsManager.Instance.GetItem(_itemToSpawn));
            PlayerInteractions.Instance.GetNewItem(obejctToReturn);*/
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
