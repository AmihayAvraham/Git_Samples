using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RopeMan.Store
{
   public class ItemSlotUI : MonoBehaviour
   {
      [SerializeField] private TextMeshProUGUI _priceText;
      [SerializeField] private Image _icon;
      [SerializeField] private GameObject[] _toggledIcons;

      private int _price;
      private Item _item;
      private int _slot;

      public void Setup(StoreItem storeItem,Item item, int slot)
      {
         _price = storeItem.price;
         _priceText.text = _price.ToString();
         _icon.sprite = storeItem.icon;
         _item = item;
         _item.gameObject.SetActive(false);
         _slot = slot;
      }

      public Vector3 GetItemPosition() => _item.transform.position;

      public int TogglePurchased()
      {
         foreach (var icon in _toggledIcons)
         {
            icon.SetActive(!icon.activeInHierarchy);
         }

         GetComponent<Button>().enabled = false;
         _item.gameObject.SetActive(true);
         return _price;
      }

      public void PurchaseButton()
      {
         StoreManager.Instance.PurchaseItem(_slot);
      }
   }
}
