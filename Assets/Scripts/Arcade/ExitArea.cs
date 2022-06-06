using UnityEngine;

namespace Arcade
{
    public class ExitArea : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("meteor"))
            {
                var sperm = other.GetComponent<SpermController>();
                sperm.separated = true;
            }
        }
    }
}