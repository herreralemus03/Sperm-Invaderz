using UnityEngine;
using Common;

namespace Arcade
{
    public class ShootController : MonoBehaviour
    {
        public float shootForce = 1;
        public SpriteRenderer sprite;
        private static readonly int Damaged = Animator.StringToHash("Damaged");

        private void Start()
        {
            MoveUp();
        }

        private void MoveUp()
        {
            LeanTween.moveLocalY(gameObject, DataManagerUtility.GetScreenHeight() * 1.5f, 1f)
                .setOnComplete(() => DestroyShoot(false));
        }

        private void DestroyShoot(bool destroyedSperm)
        {
            destroySperm = destroyedSperm;
            Destroy(gameObject);
        } 
        private void FixedUpdate()
        {
            transform.Rotate(Vector3.back);
            /*if (transform.position.y > DataManagerUtility.GetScreenHeight() * 0.9f)
                PlayerController.Instance.canDoExtraScore = false;*/
        }

        public bool destroySperm;
        private void OnBecameInvisible()
        {
            if(!destroySperm && PlayerController.Instance) PlayerController.Instance.CanDoExtraScore = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                if (other.CompareTag("meteor"))
                {
                    var sperm = other.gameObject.GetComponent<SpermController>();
                    sperm.Life -= shootForce;
                    if (sperm.Life <= 0)
                    {
                        if (GameController.Instance.gameState == GameState.Ended) return;
                        GameController.Instance.StartCoroutine(GameController.Instance.DestroySperm(sperm, 0f, "shoot controller: on trigger enter 2d"));
                        PlayerController.Instance.CanDoExtraScore = true;
                        ScoreController.Instance.InstantiateScore(transform.position, 1);
                    }
                    else
                    {
                        sperm.damageAnimation.SetTrigger(Damaged);
                        if (AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
                    }
                }
                DestroyShoot(true);
            }
        }
    }
}