using Common;
using UnityEngine;

namespace Arcade
{
    public class CondomController : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite condomDamageSprite, condomNormalSprite;
        public GameObject particlesWithStars;
        public GameObject particlesWithoutStars;

        private int Hits
        {
            get => mHits;
            set
            {
                mHits = value;
                if (value >= 2)
                {
                    DestroyCondom();
                    return;
                }

                if (value >= 1)
                {
                    ChangeSkinToDamage();
                }
            }
        }

        private void ChangeSkinToDamage()
        {
            Instantiate(particlesWithoutStars, transform.position, Quaternion.identity);
            spriteRenderer.sprite = condomDamageSprite;
        }

        private int mHits;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Hits++;
            if(AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
            PlayerController.Instance.CanDoExtraScore = false;
            Destroy(other.gameObject);
        }

        private void DestroyCondom()
        {
            if(AudioManager.Instance) AudioManager.Instance.explosionAudio.Play();
            Instantiate(particlesWithStars, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}