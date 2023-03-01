using UnityEngine;

namespace Unity.FPS.LevelEditor
{
    public class EditorController : MonoBehaviour
    {
        private Transform _transform;
        [SerializeField] private Vector2 _ScrollBounds;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _moveSpeed;
        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            //Controls the camera movement when pushing the middle mouse button
            if (Input.GetMouseButton(2))
            {
                HandleMovement();
            }

            //Controls the camera zoom when wheeling the middle mouse button and holding leftCtrl
            if (Input.mouseScrollDelta.magnitude != 0&&Input.GetKey(KeyCode.LeftControl))
            {
                HandleZoom();
            }
        }

        private void HandleMovement()
        {
            Vector3 direction = new Vector3();
            direction.x = (-1)*Input.GetAxis("Mouse X");
            direction.y = Input.GetAxis("Mouse Y");
            
            _transform.Translate(direction*_moveSpeed);
        }

        private void HandleZoom()
        {
            if (_camera.orthographicSize < _ScrollBounds.x)
            {
                _camera.orthographicSize = _ScrollBounds.x;
            }
            
            if (_camera.orthographicSize > _ScrollBounds.y)
            {
                _camera.orthographicSize = _ScrollBounds.y;
            }
            _camera.orthographicSize -= Input.mouseScrollDelta.y;
        }
    }
}

