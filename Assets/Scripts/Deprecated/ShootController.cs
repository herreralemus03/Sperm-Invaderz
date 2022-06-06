/*using Common;
using Invasion;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public bool modified;
    public float shootForce, earns;
    public SpriteRenderer sprite;
    private void Start()
    {
        MoveUp();
        if (GameController.Instance.gameMode == GameMode.Arcade || GameController.Instance.gameMode == GameMode.Endless)
        {
            shootForce = 200f;
            return;
        }
        ChangeShootProperties();
        earns = DataManagerUtility.GetShotEarns(SpermSpawnController.Instance.test ? SpermSpawnController.Instance.shootEarn : PlayerPrefs.GetInt("earns", 1));
    }
    private void ChangeShootProperties()
    {
        var aux = DataManagerUtility.GetShotForce(PlayerPrefs.GetInt("power", 1));
        if (PlayerController.Instance.PowerType == PowerType.Force || PlayerController.Instance.PowerType == PowerType.Everything)
        {
            shootForce = aux * 2f;
        } else if (PlayerController.Instance.PowerType == PowerType.Speed)
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
        LeanTween.moveLocalY(gameObject, SpermSpawnController.Instance.cameraHeight * 1.5f, 1f).setOnComplete(DestroyShoot);
    }
    private void FixedUpdate()
    {
        transform.Rotate(Vector3.back);
        if(transform.position.y > SpermSpawnController.Instance.cameraHeight*0.9f) PlayerController.Instance.canDoExtraScore = false; 
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            if (other.CompareTag("boss"))
            {
                if (!BossController.Instance.isDoingSpecialMovement)
                {
                    OvumController.Instance.Fertility-=1f;
                    if (AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
                    BossController.Instance.secondAnimation.SetTrigger("Damaged");
                }
                else
                {
                    if(AudioManager.Instance) AudioManager.Instance.missAudio.Play();
                }
            }
            if (other.CompareTag("meteor"))
            {
                var sperm = other.gameObject.GetComponent<SpermController>();
                sperm.Life-=shootForce;
                SpermSpawnController.Instance.PlaySplashParticles(transform.position);
                if(sperm.Life <= 0)
                {
                    if(GameController.Instance.gameState == GameState.Ended) return;
                    sperm.isDestroyed = true;
                    if (GameController.Instance.gameMode == GameMode.Arcade || 
                        GameController.Instance.gameMode == GameMode.Endless)
                    {
                        SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(sperm, 0f, "shoot controller: on trigger enter 2d"));
                        if (GameController.Instance.gameMode == GameMode.Endless) IncreaseLevelForInfiniteMode();
                        if (GameController.Instance.gameMode == GameMode.Arcade)
                        {
                            PlayerController.Instance.canDoExtraScore = true;
                        }
                        ScoreController.Instance.InstantiateScore(transform.position, 1);
                    }
                    else
                    {
                        ScoreController.Instance.InstantiateScoreText(transform.position, Mathf.RoundToInt(earns));
                        SpawnOther(sperm, true);
                    }
                }
                else
                {
                    sperm.damageAnimation.SetTrigger("Damaged");
                    if(AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
                }
            }

            if (PlayerController.Instance.PowerType == PowerType.Intangible || PlayerController.Instance.PowerType == PowerType.Everything) return;
            DestroyShoot();
        }
    }
    private void DestroyShoot()
    {
        Destroy(gameObject);
    }
    private void IncreaseLevelForInfiniteMode()
    {
        SpermSpawnController.Instance.rate++;
        if(SpermSpawnController.Instance.rate <= SpermSpawnController.Instance.limitRate) return;
        SpermSpawnController.Instance.limitRate+=3;
        SpermSpawnController.Instance.rate -= Mathf.RoundToInt(SpermSpawnController.Instance.rate*0.5f);
        SpermSpawnController.Instance.CancelInvoke("InstantiateSpermInInfiniteMode");
        PlayerController.Instance.CancelInvoke("Shoot");
        SpermSpawnController.Instance.levelForInfiniteMode++;
        SpermSpawnController.Instance.InvokeRepeating("InstantiateSpermInInfiniteMode", 0f, DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(SpermSpawnController.Instance.levelForInfiniteMode));
        PlayerController.Instance.InvokeRepeating("Shoot", 0f, DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(SpermSpawnController.Instance.levelForInfiniteMode)/2f);
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
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(SpermSpawnController.Instance.spermsToInstance.Find(sperm => sperm.type == SpermsType.Normal), position));
                break;
            case SpermsType.Fastersperm:
                if (GameController.Instance.gameState != GameState.Playing) break;
                spawnOther = true;
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(SpermSpawnController.Instance.spermsToInstance.Find(sperm => sperm.type == SpermsType.Normal), position));
                break;
            case SpermsType.Influencer:
                if (GameController.Instance.gameState != GameState.Playing) break;
                spawnOther = true;
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(SpermSpawnController.Instance.spermsToInstance.Find(sperm => sperm.type == SpermsType.Protected), position));
                break;
            case SpermsType.Ninja:
                if (GameController.Instance.gameState != GameState.Playing) break;
                spawnOther = true;
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(SpermSpawnController.Instance.spermsToInstance.Find(sperm => sperm.type == SpermsType.Normal), position - (Vector3.right * .25f)));
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(SpermSpawnController.Instance.spermsToInstance.Find(sperm => sperm.type == SpermsType.Normal), position + (Vector3.right * .25f)));
                break;
        }

        SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(spermController, 0f, "shoot controller: spawn other", destroyedByPlayer, spawnOther));
    }
}*/