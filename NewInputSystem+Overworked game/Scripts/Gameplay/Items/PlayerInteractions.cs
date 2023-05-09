using System;
using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public class PlayerInteractions : MonoBehaviour
    {
        private WorkStation _workstationIdentified;
        private ItemObject _currentItem;
        private bool _carryingItem;
        private Transform _transform;
        [SerializeField] private float MaxReach;
        [SerializeField] private LayerMask _layerTarget;
        [SerializeField] private Transform _itemParent;
        [SerializeField] private InputHandler _input;

        public static PlayerInteractions Instance;

        private void Awake()
        {
            _transform = transform;
            Instance = this;
        }

        private void Update()
        {
            HandleRay();

            if (_input.Action)
            {
                DoAction();
            }
            
            if (_input.Pick)
            {
                HandlePickUpDownButton();
            }
        }


        private void HandleRay()
        {
            RaycastHit hit;

            if (!Physics.Raycast(_transform.position, _transform.TransformDirection(Vector3.forward), out hit,
                    MaxReach, _layerTarget))
            {
                _workstationIdentified = null;
                Debug.DrawRay(_transform.position, _transform.TransformDirection(Vector3.forward) * MaxReach,
                    Color.red);
            }
            else
            {
                Debug.DrawRay(_transform.position, _transform.TransformDirection(Vector3.forward) * hit.distance,
                    Color.green);

                if (hit.collider.gameObject.GetComponent<WorkStation>() != null)
                    {
                        _workstationIdentified = hit.collider.gameObject.GetComponent<WorkStation>();
                    }
            }
        }

        public void DoAction()
        {
            if (_workstationIdentified == null||_currentItem!=null)
            {
                return;
            }
            _workstationIdentified.DoAction();
        }

        public void GetNewItem(ItemObject item)
        {
            if (_currentItem != null)
            {
                return;
            }
            _currentItem = item;
            Transform transform1;
            (transform1 = _currentItem.transform).SetParent(_itemParent);
            transform1.localPosition = Vector3.zero;
        }

        public void HandlePickUpDownButton()
        {
            if (_workstationIdentified == null)
            {
                return;
            }
            

            if (_currentItem == null&&_workstationIdentified.HasItem()||_workstationIdentified.GetComponent<WorkStation_ItemBox>()!=null)
            {
                PickUpItem();
            }
            else if(_currentItem != null&&!_workstationIdentified.HasItem())
            {
                PutDownItem();
            }
            else if(_currentItem.GetCombineKey()!="")
            {
                if (_currentItem.GetCurrentKey() != _workstationIdentified._snappedItem.GetCurrentKey() &&
                    _currentItem.GetCombineKey() == _workstationIdentified._snappedItem.GetCombineKey())
                {
                    CombineItems();
                }
            }
        }
        private void PickUpItem()
        {
            if (_currentItem != null)
            {
                return;
            }
            _currentItem = _workstationIdentified.Pickup();
            Transform transform1;
            (transform1 = _currentItem.transform).SetParent(_itemParent);
            transform1.localPosition = Vector3.zero;
        }
        private void PutDownItem()
        {
            _workstationIdentified.PutDown(_currentItem);
            _currentItem = null;
        }

        private void CombineItems()
        {
            _workstationIdentified.CombineItems(_currentItem);
            _currentItem.gameObject.SetActive(false);
            _currentItem = null;  
        }
    }
}
