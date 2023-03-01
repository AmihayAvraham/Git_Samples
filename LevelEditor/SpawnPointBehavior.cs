using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class SpawnPointBehavior : MonoBehaviour
    {
        private bool _foundPlayer;
        //Move player to spawn point on start
        private void Update()
        {
            if (!_foundPlayer)
            {
                if (FindObjectOfType<PlayerCharacterController>() == null)
                {
                    return;
                }
                FindObjectOfType<PlayerCharacterController>().transform.position = transform.position;
                _foundPlayer = true;
            }
        }
    }
}
