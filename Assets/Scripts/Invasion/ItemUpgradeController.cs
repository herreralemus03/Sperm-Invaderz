using UnityEngine;

namespace Invasion
{
    public class ItemUpgradeController : MonoBehaviour
    {
        public PowerType powerType;
        public Rigidbody2D rb;
        private void Start()
        {
            rb.velocity = Vector2.down * 2f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController.Instance.GivePower(powerType);
                Destroy(gameObject);       
            }
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}