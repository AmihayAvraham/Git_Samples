using UnityEngine;
using UnityEngine.InputSystem;

namespace Nokobot.Mobile.Overworked
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _playerInputActionAsset;
        [SerializeField] private string _playerNumber;

        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _pickAction;
        private InputAction _doAction;
        public Vector2 InputValues;

        private float _speed = 1f;
        public float Factor = 1f;
        public bool Action;
        public bool Pick;

        private void Awake()
        {
            InputActionMap actionMap = _playerInputActionAsset.FindActionMap("Player " + _playerNumber);
            actionMap.Enable();

            _moveAction = actionMap.FindAction("Move");
            _sprintAction = actionMap.FindAction("Sprint");
            _doAction = actionMap.FindAction("Action");
            _pickAction = actionMap.FindAction("Pick");
        }


        private void Update()
        {
            Factor = _sprintAction.IsPressed() ? 2f : 1f;
            InputValues = _moveAction.ReadValue<Vector2>();
            Action = _doAction.triggered;
            Pick = _pickAction.triggered;
        }
    }
}
