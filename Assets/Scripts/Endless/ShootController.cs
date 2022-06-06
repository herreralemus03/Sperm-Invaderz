using UnityEngine;
using Common;

namespace Endless
{
    public class ShootController : MonoBehaviour
    {
        public float shootForce;
        public SpriteRenderer sprite;
        private static readonly int Damaged = Animator.StringToHash("Damaged");

        private void Start()
        {
            MoveUp();
        }

        private void MoveUp()
        {
            LeanTween.moveLocalY(gameObject, DataManagerUtility.GetScreenHeight() * 1.5f, 1f)
                .setOnComplete(DestroyShoot);
        }

        private void DestroyShoot()
        {
            Destroy(gameObject);
        } 
        
        private void FixedUpdate()
        {
            transform.Rotate(Vector3.back);
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
                        sperm.isDestroyed = true;
                        GameController.Instance.StartCoroutine(
                            GameController.Instance.DestroySperm(sperm, 0f,
                                "shoot controller: on trigger enter 2d"));
                        IncreaseLevelForInfiniteMode();
                        ScoreController.Instance.InstantiateScore(transform.position, 1);
                        
                    }
                    else
                    {
                        sperm.damageAnimation.SetTrigger(Damaged);
                        if (AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
                    }
                }

                DestroyShoot();
            }
        }
        private static void IncreaseLevelForInfiniteMode()
        {
            GameController.Instance.rate++;
            if (GameController.Instance.rate <= GameController.Instance.limitRate) return;
            GameController.Instance.limitRate += 3;
            GameController.Instance.rate -= Mathf.RoundToInt(GameController.Instance.rate * 0.5f);
            GameController.Instance.CancelInvoke(nameof(GameController.InstantiateSpermInInfiniteMode));
            PlayerController.Instance.CancelInvoke(nameof(PlayerController.Shoot));
            GameController.Instance.level++;
            GameController.Instance.InvokeRepeating(nameof(GameController.Instance.InstantiateSpermInInfiniteMode), 0f,
                DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(GameController.Instance
                    .level));
            PlayerController.Instance.InvokeRepeating(nameof(PlayerController.Shoot), 0f,
                DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(GameController.Instance
                    .level) * 0.4f);
        }
    }
}