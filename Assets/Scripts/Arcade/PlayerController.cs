using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Common;
using Invasion;

namespace Arcade
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Shoot1 = Animator.StringToHash("shoot");
        private static readonly int Show = Animator.StringToHash("show");
        private static readonly int Hide = Animator.StringToHash("hide");

        public static PlayerController Instance { private set; get; }

        [Header("Spawn Object")] public ShootController
            shoot;

        [Header("Other")] public Animator shootAnimation, playerAnimator, knockOutAnimator;

        private float
            mAngle,
            mMoveSpeed;

        private Vector3
            mMousePosition,
            mVelocity,
            mNewPos,
            mOldPos;

        public GameObject
            trigger0,
            trigger1,
            trigger2;

        public GameObject model3D;

        public Sprite
            shuriken,
            pill,
            multishot,
            power,
            speed,
            phantom;

        private PowerType
            mPowerType;

        public bool
            powerIsOn,
            isHitByBoss,
            canDoExtraScore,
            cancelExtraScoreRunning;

        public int
            powerDuration,
            spermsDestroyedConsecutive;


        public bool CanDoExtraScore
        {
            get => canDoExtraScore;
            set
            {
                if (value)
                {
                    if (spermsDestroyedConsecutive <= 8) spermsDestroyedConsecutive++;
                }
                else
                {
                    spermsDestroyedConsecutive = 0;
                }

                canDoExtraScore = value;
            }
        }

        private float mScreenHeight;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            mScreenHeight = DataManagerUtility.GetScreenHeight();
        }

        private void Update()
        {
            Rotate();
        }

        private void Rotate()
        {
            mNewPos = transform.position;
            var media = (mNewPos - mOldPos);
            mVelocity = media / Time.deltaTime;
            mOldPos = mNewPos;
            mNewPos = transform.position;
            model3D.transform.Rotate(Vector3.up, mVelocity.x * 5f);
            PlaySwimSound();
        }

        private void PlaySwimSound()
        {
            if (mVelocity.x <= 0 || !AudioManager.Instance) return;

            if (mVelocity.x > 90)
            {
                if (AudioManager.Instance.swimFast1Audio.isPlaying) return;
                AudioManager.Instance.swimFast1Audio.Play();
            }
            else if (mVelocity.x > 80)
            {
                if (AudioManager.Instance.swimFast2Audio.isPlaying) return;
                AudioManager.Instance.swimFast2Audio.Play();
            }
            else if (mVelocity.x > 60)
            {
                if (AudioManager.Instance.swimMedium1Audio.isPlaying) return;
                AudioManager.Instance.swimMedium1Audio.Play();
            }
            else if (mVelocity.x > 40)
            {
                if (AudioManager.Instance.swimMedium2Audio.isPlaying) return;
                AudioManager.Instance.swimMedium2Audio.Play();
            }
            else if (mVelocity.x > 20)
            {
                if (AudioManager.Instance.swimSoft1Audio.isPlaying) return;
                AudioManager.Instance.swimSoft1Audio.Play();
            }
            else
            {
                if (AudioManager.Instance.swimSoft2Audio.isPlaying) return;
                AudioManager.Instance.swimSoft2Audio.Play();
            }
        }

        private void FixedUpdate()
        {
            mMousePosition = Input.mousePosition;
            mMousePosition = Camera.main.ScreenToWorldPoint(mMousePosition);
            if (mMousePosition.y > mScreenHeight / 2) return;
            if (Input.GetMouseButton(0))
                transform.position = Vector2.Lerp(transform.position,
                    new Vector3(mMousePosition.x, transform.position.y), 0.2f);
        }

        public List<GameObject> shoots = new List<GameObject>();
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void Shoot()
        {
            if (GameController.Instance.gameState != GameState.Playing) return;
            if (!Input.GetMouseButton(0)) return;
            var shoot = Instantiate(this.shoot, trigger0.transform.position, Quaternion.identity);
            shoot.sprite.sprite = pill;
            shootAnimation.SetTrigger(Shoot1);
            if (AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
        }

        public bool knockedOut;
        private static readonly int Intangible = Animator.StringToHash("Intangible");

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(knockedOut) return;
            if (other.gameObject.CompareTag("shoot") ||
                other.gameObject.CompareTag("item")) return;

            if (!other.gameObject.CompareTag("meteor")) return;
            if(AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
            GameController.Instance.StartCoroutine(GameController.Instance.DestroySperm(other.GetComponent<SpermController>(), 0f, "", false));
                
            if (ScoreArcadeMode.Instance.playerLife > 1)
            {
                if(GameController.Instance.gameState == GameState.Playing) ScoreArcadeMode.Instance.playerLife--;
                LifeController.Instance.amount.text = $"{ScoreArcadeMode.Instance.playerLife}";
                knockedOut = true;
                knockOutAnimator.gameObject.SetActive(true);
                playerAnimator.SetTrigger(Intangible);
                StartCoroutine(ExitKnockOut());
                if(AudioManager.Instance) AudioManager.Instance.dizzyBirds.Play();
            }
            else
            {
                if(GameController.Instance.gameState != GameState.Playing) return; 
                GameController.Instance.SetGameState(GameState.Ended);
            }
            LifeController.Instance.amount.text = $"{ScoreArcadeMode.Instance.playerLife}";

            /*
            if (GameController.Instance.gameState != GameState.Playing) return;
            if (AudioManager.Instance) AudioManager.Instance.playerCollisionAudio.Play();
            if (AudioManager.Instance) AudioManager.Instance.loseAudio.Play();
            if (isHitByBoss) return;
            if (other.gameObject.CompareTag("boss")) isHitByBoss = true;
            GameController.Instance.SetGameState(GameController.Instance.CheckIfCanRetry()
                ? GameState.Retry
                : GameState.Ended);*/
        }
        private IEnumerator ExitKnockOut()
        {
            var knockOutDuration = 2f;
            while (knockOutDuration > 0f)
            {
                yield return new WaitForSeconds(1f);
                knockOutDuration--;
            }
            knockedOut = false;
            knockOutAnimator.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            Instance = null;
        }

    }
}