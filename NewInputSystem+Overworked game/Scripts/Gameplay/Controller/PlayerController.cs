using System;
using UnityEngine;

namespace Nokobot.Mobile.Overworked
{
    public class PlayerController : MonoBehaviour
    {
        private InputHandler _input;
        private Transform _transform;

        [Header("Control Parameters")] 
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotSpeed;
        [SerializeField] private Transform _rotPivot;

        private void Start()
        {
            _transform = transform;
            _input = GetComponent<InputHandler>();
        }

        private void Update()
        {
            _HandleMovement();
        }

        private void _HandleMovement()
        {
            Vector3 direction = new Vector3(_input.InputValues.x, 0f, _input.InputValues.y).normalized;
            _transform.Translate(direction * Time.deltaTime*_moveSpeed*_input.Factor);

            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                _rotPivot.rotation = Quaternion.RotateTowards(_rotPivot.rotation, toRotation, Time.deltaTime*_rotSpeed);
            }
            
        }
    }
}
