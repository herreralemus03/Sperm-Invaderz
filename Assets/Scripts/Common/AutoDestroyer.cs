using UnityEngine;

namespace Common
{
    public class AutoDestroyer : MonoBehaviour
    {
        public float destroyTime = 2f;
        private void Start()
        {
            Destroy(gameObject, destroyTime);
        }
    }
}