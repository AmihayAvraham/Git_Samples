using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace RopeMan.Store
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private TextMeshProUGUI _gemText;
        
        [SerializeField] private ItemSlotUI _baseSlot;
        [SerializeField] private Item[] _itemObject;

        private int _playerCoins;
        private int _playerGems;

        private PurchaseEffect _purchaseEffect;
        private StoreItem[] _storeItems;
        private List<ItemSlotUI> _storeSlots;

        public bool ResetPrefs;
        
        public static StoreManager Instance;
        
        public static event Action<Vector3> OnPurchaseEvent;
        private void Awake()
        {
            Instance = this;
            _purchaseEffect = GetComponent<PurchaseEffect>();
            _storeItems = Resources.LoadAll<StoreItem>("StoreItems");
            _storeSlots = new List<ItemSlotUI>();
        }
        private void Start()
        {
            for (var i = 0; i < _storeItems.Length; i++)
            {
                ItemSlotUI storeSlot = Instantiate(_baseSlot,_baseSlot.transform.parent.transform);
                storeSlot.Setup(_storeItems[i],GetItemByID(_storeItems[i].ID),i);
                storeSlot.gameObject.SetActive(true);
                _storeSlots.Add(storeSlot);
            }

            //**** replace with player coins and gems value method here -----------------------------------<<<<
            _playerCoins = 2000;
            _playerGems = 150;
            //
            _coinText.text = _playerCoins.ToString();
            _gemText.text = _playerGems.ToString();

            if (ResetPrefs)
            {
                foreach (var item in _storeItems)
                {
                    PlayerPrefs.SetInt(item.ID,0);
                }
            }
            RestorePurchases();
        }

        public void PurchaseItem(int slot)
        {
            //**** replace with coin handler method to reduce coins -----------------------------------<<<<
            _playerCoins -= _storeSlots[slot].TogglePurchased();
            _coinText.text = _playerCoins.ToString();
            //
            PlayerPrefs.SetInt(_storeItems[slot].ID,1);
            OnPurchaseEvent?.Invoke(_storeSlots[slot].GetItemPosition()+Vector3.up*3);
        }

        public void RestorePurchases()
        {
            for (int i=0;i<_storeItems.Length;i++)
            {
                if (PlayerPrefs.GetInt(_storeItems[i].ID, 0) == 1)
                {
                    _storeSlots[i].TogglePurchased();
                }
            }
        }

        private Item GetItemByID(string ID)
        {
            for (int i = 0; i <= _itemObject.Length; i++)
            {
                if (_itemObject[i].GetID() == ID)
                {
                    return _itemObject[i];
                }
            }

            return null;
        }
        
        
    }
}
