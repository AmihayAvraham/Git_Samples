using DG.Tweening;
using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public class OrderUIBehaviour : MonoBehaviour
    {
        private Transform _transform;
        [SerializeField] private Transform _orderTransform;
        [SerializeField] private Transform _componentTransform;

        private void Awake()
        {
            _transform = transform;
            _orderTransform.DOLocalMove(Vector3.zero, 1f);
            Invoke(nameof(CloseOrder), 5f);
        }

        public void CloseOrder()
        {
            _transform.DOScale(Vector3.zero, 0.1f);
            Invoke(nameof(TurnOffOrder), 1f);
        }

        private void TurnOffOrder()
        {
            gameObject.SetActive(false);
        }
    }
}
