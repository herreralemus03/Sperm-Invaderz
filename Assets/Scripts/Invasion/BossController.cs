using System.Collections;
using Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Invasion
{
    public class BossController : MonoBehaviour
    {
        public float spawnRate;
        public Animator animator;
        public Animator secondAnimation;
        public bool isDoingSpecialMovement = true;
        public Vector3 initialPosition;
        public BossTrigger bossTrigger;
        public SpriteRenderer damageArea;
        public IEnumerator bossMoveLeft, bossMoveMiddle, bossMoveRight, fertilize;
        public static BossController Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            initialPosition = transform.position;
            var level = PlayerPrefs.GetInt("level", 10);
            spawnRate = DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(level);

            Invoke("StartBossMovements", 3f);
            Invoke("PlaySwordSound", 1f);
        }

        private void PlaySwordSound()
        {
            if(AudioManager.Instance) AudioManager.Instance.swordAudio.Play();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("ovum"))
            {
                OvumController.Instance.IncrementFertility();
            }
        }
        private bool mExit;
        private void FixedUpdate()
        {
            if(isDoingSpecialMovement)
            {
                damageArea.color = Color32.Lerp(damageArea.color, new Color32(103, 103, 103, 255), 0.125f);
                return;
            }
            damageArea.color = Color32.Lerp(damageArea.color, new Color32(170, 170, 170, 255), 0.125f);
            FollowPlayer();
        }

        public void StartBossMovements()
        {
            StartSpermSpawn();
        }
        public void StartSpermSpawn()
        {
            isDoingSpecialMovement = true;
            StartCoroutine(SpawnSpermMovementRight());
        }
    
        public IEnumerator SpawnSpermMovementRight()
        {
            if(CheckIfGameIsPlaying())
            {
                HideBoss();
                yield break;
            }
            isDoingSpecialMovement = true;
            LeanTween.moveX(gameObject, DataManagerUtility.GetScreenWidth() *.65f, 1f).setOnComplete(bossTrigger.SpawnSperm);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(SpawnSpermMovementMiddle());
        }

        private bool CheckIfGameIsPlaying()
        {
            return (GameController.Instance.gameState == GameState.Ended || GameController.Instance.gameState == GameState.Cleared);
        }

        public void HideBoss()
        {
            Debug.Log("BOSS LOSS");
            LeanTween.moveY(gameObject, DataManagerUtility.GetScreenHeight() * 2, 2f);
        }
    
        public IEnumerator SpawnSpermMovementLeft()
        {
            if(CheckIfGameIsPlaying())
            {
                HideBoss();
                yield break;
            }
            isDoingSpecialMovement = true;
            LeanTween.moveX(gameObject, -DataManagerUtility.GetScreenWidth() *.65f, 1f).setOnComplete(bossTrigger.SpawnSperm);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(BackToInitialPosition());
        }
    
        public IEnumerator SpawnSpermMovementMiddle()
        {
            if(CheckIfGameIsPlaying())
            {
                HideBoss();
                yield break;
            }
            isDoingSpecialMovement = true;
            LeanTween.moveX(gameObject, 0f, 1f).setOnComplete(bossTrigger.SpawnSperm);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(SpawnSpermMovementLeft());
        }

        public IEnumerator FertilizeMovement()
        {
            if(CheckIfGameIsPlaying())
            {
                HideBoss();
                yield break;
            }
            isDoingSpecialMovement = true;
            yield return new WaitForSeconds(1f);
            if(AudioManager.Instance) AudioManager.Instance.dashAudio.Play();
            LeanTween.moveY(gameObject, 0f, 1f).setOnComplete(BackToInitialPositionM);
            yield return new WaitForSeconds(2f);
        }

        public IEnumerator BackToInitialPosition()
        {
            LeanTween.moveLocal(gameObject, initialPosition, 3f).setOnComplete(AllowFollowPlayer);
            yield return new WaitForSeconds(spawnRate * waitTimeToChangeMovement);
            isDoingSpecialMovement = false;
            ChangeMovement();
        }

        public void AllowFollowPlayer()
        {
            isDoingSpecialMovement = false;
        }
        public void BackToInitialPositionM()
        {
            StartCoroutine(BackToInitialPosition());
        }
        public void ChangeMovement()
        {
            var index = Random.Range(1, 3);
            switch (index)
            {
                case 1:
                    StartSpermSpawn();
                    break;
                default:
                    StartCoroutine(FertilizeMovement());
                    break;
            }
        }
    
        public Vector3 playerPosition;
        private void FollowPlayer()
        {
            if(isDoingSpecialMovement || CheckIfGameIsPlaying()) return;
            playerPosition = new Vector3(PlayerController.Instance.transform.position.x, transform.position.y);
            transform.position = Vector3.Lerp(transform.position, playerPosition, 0.016f);
            distance = Vector3.Distance(new Vector3(playerPosition.x, 0f), new Vector3(transform.position.x, 0f));
            if(distance <= 0.2 && !spermSpawned) StartCoroutine(SpawnSperm());
        }

        public bool spermSpawned;
        public float distance = 10f;
        private IEnumerator SpawnSperm()
        {
            spermSpawned = true;
            bossTrigger.SpawnSperm();
            yield return new WaitForSeconds(spawnRate*waitTimeToSpawnSperm);
            spermSpawned = false;
        }

        public float waitTimeToSpawnSperm = 5f, waitTimeToChangeMovement = 35f;
        private void OnDestroy()
        {
            Instance = null;
        }
    }
}