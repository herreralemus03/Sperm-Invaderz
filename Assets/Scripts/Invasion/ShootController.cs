using UnityEngine;
using Common;

namespace Invasion
{
    public class ShootController : MonoBehaviour
    {
        public float shootForce;
        public float earns;
        public SpriteRenderer sprite;
        private static readonly int Damaged = Animator.StringToHash("Damaged");

        private void Start()
        {
            MoveUp();
            ChangeShootProperties();
            earns = DataManagerUtility.GetShotEarns(PlayerPrefs.GetInt("earns", 1));
        }

        private void ChangeShootProperties()
        {
            var aux = DataManagerUtility.GetShotForce(PlayerPrefs.GetInt("power", 1));
            if (PlayerController.Instance.PowerType == PowerType.Force ||
                PlayerController.Instance.PowerType == PowerType.Everything)
            {
                shootForce = aux * 2f;
            }
            else if (PlayerController.Instance.PowerType == PowerType.Speed)
            {
                shootForce = aux * .5f;
            }
            else
            {
                shootForce = aux;
            }
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

        private void HandleOnBossCollision(Collider2D other)
        {
            SpermSpawnController.Instance.CheckIfClearGame();
            if (!BossController.Instance.isDoingSpecialMovement)
            {
                OvumController.Instance.Fertility-=1f;
                if (AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
                BossController.Instance.secondAnimation.SetTrigger(Damaged);
            }
            else
            {
                if(AudioManager.Instance) AudioManager.Instance.missAudio.Play();
            }
        }
        private void HandleOnSpermShot(Component other)
        {
            var sperm = other.gameObject.GetComponent<SpermController>();
            sperm.Life -= shootForce;
            SpermSpawnController.Instance.PlaySplashParticles(transform.position);

            if (sperm.Life <= 0)
            {
                sperm.isDestroyed = true;
                ScoreController.Instance.InstantiateScoreText(transform.position, Mathf.RoundToInt(earns));
                SpawnOther(sperm, true);
            }
            else
            {
                sperm.damageAnimation.SetTrigger(Damaged);
                if (AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;
            if (other.CompareTag("meteor")) HandleOnSpermShot(other);
            if (other.CompareTag("boss")) HandleOnBossCollision(other);
                
            if (PlayerController.Instance.PowerType == PowerType.Penetrating ||
                PlayerController.Instance.PowerType == PowerType.Everything) return;
            DestroyShoot();
        }
        private void SpawnOther(SpermController spermController, bool destroyedByPlayer = false)
        {
            var position = spermController.transform.position;
            var spawnOther = false;
            switch (spermController.type)
            {
                case SpermsType.Protected:
                    if (GameController.Instance.gameState != GameState.Playing) break;
                    spawnOther = true;
                    SpermSpawnController.Instance.StartCoroutine(
                        SpermSpawnController.Instance.InstantiateSpermWithRandomLife(
                            SpermSpawnController.Instance.spermsToInstance.Find(
                                sperm => sperm.type == SpermsType.Normal), position));
                    break;
                case SpermsType.FasterSperm:
                    if (GameController.Instance.gameState != GameState.Playing) break;
                    spawnOther = true;
                    SpermSpawnController.Instance.StartCoroutine(
                        SpermSpawnController.Instance.InstantiateSpermWithRandomLife(
                            SpermSpawnController.Instance.spermsToInstance.Find(
                                sperm => sperm.type == SpermsType.Normal), position));
                    break;
                case SpermsType.Protected2:
                    if (GameController.Instance.gameState != GameState.Playing) break;
                    spawnOther = true;
                    SpermSpawnController.Instance.StartCoroutine(
                        SpermSpawnController.Instance.InstantiateSpermWithRandomLife(
                            SpermSpawnController.Instance.spermsToInstance.Find(sperm =>
                                sperm.type == SpermsType.Protected), position));
                    break;
                case SpermsType.Ninja:
                    if (GameController.Instance.gameState != GameState.Playing) break;
                    spawnOther = true;
                    SpermSpawnController.Instance.StartCoroutine(
                        SpermSpawnController.Instance.InstantiateSpermWithRandomLife(
                            SpermSpawnController.Instance.spermsToInstance.Find(
                                sperm => sperm.type == SpermsType.Normal), position - (Vector3.right * .25f)));
                    SpermSpawnController.Instance.StartCoroutine(
                        SpermSpawnController.Instance.InstantiateSpermWithRandomLife(
                            SpermSpawnController.Instance.spermsToInstance.Find(
                                sperm => sperm.type == SpermsType.Normal), position + (Vector3.right * .25f)));
                    break;
            }

            SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(spermController, 0f,
                "shoot controller: spawn other", destroyedByPlayer, spawnOther));
        }
    }
}