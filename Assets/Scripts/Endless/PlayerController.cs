using System.Collections;
using Common;
using UnityEngine;

namespace Endless
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Shoot1 = Animator.StringToHash("shoot");
        private static readonly int Intangible = Animator.StringToHash("Intangible");
        
        public Color normalColor;
        public static PlayerController Instance { private set; get; }

        public ShootController laser;
        public Animator shootAnimation, knockOutAnimator, playerAnimator;

        private float
            mAngle,
            mMoveSpeed;

        private Vector3
            mMousePosition,
            mVelocity,
            mNewPos,
            mOldPos;

        public GameObject trigger0;

        public GameObject model3D;
        public Sprite pill;

        private PowerType
            mPowerType;

        private void Awake()
        {
            if (Instance == null) Instance = this;
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
            if (mMousePosition.y > DataManagerUtility.GetScreenHeight() / 2) return;
            if (Input.GetMouseButton(0))
                transform.position = Vector2.Lerp(transform.position,
                    new Vector3(mMousePosition.x, transform.position.y), 0.2f);
        }

        public void Shoot()
        {
            if (GameController.Instance.gameState != GameState.Playing) return;
            if (!Input.GetMouseButton(0)) return;
            var shoot = Instantiate(laser, trigger0.transform.position, Quaternion.identity);
            shoot.sprite.sprite = pill;
            shootAnimation.SetTrigger(Shoot1);
            if (AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
        }

        public bool knockedOut;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(knockedOut) return;
            if (!other.gameObject.CompareTag("meteor") && !other.gameObject.CompareTag("boss")) return;
            if(AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
            if(other.gameObject.CompareTag("meteor")) GameController.Instance.StartCoroutine(GameController.Instance.DestroySperm(other.GetComponent<SpermController>(), 0f, "", false));
            knockedOut = true;
            knockOutAnimator.gameObject.SetActive(true);
            playerAnimator.SetTrigger(Intangible);
            StartCoroutine(ExitKnockOut());
            if(AudioManager.Instance) AudioManager.Instance.dizzyBirds.Play();
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