using UnityEngine;

namespace RopeMan.Store
{
    public class PurchaseEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _popParticle;
        [SerializeField] private Animator _playerAnim;
        private AudioSource _audioSource;
        
        private void Awake() => _audioSource = GetComponent<AudioSource>();
        private void OnEnable() => StoreManager.OnPurchaseEvent += PlayParticle;

        private void OnDisable() => StoreManager.OnPurchaseEvent -= PlayParticle;
        
        public void PlayParticle(Vector3 position)
        {
            _popParticle.transform.position = position;
            _popParticle.Play();
            _audioSource.Play();
            _playerAnim.SetTrigger("spin/dance");
        }
    }
}
