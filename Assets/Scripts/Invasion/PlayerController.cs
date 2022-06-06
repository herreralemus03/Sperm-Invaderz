using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Invasion
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Shoot1 = Animator.StringToHash("shoot");
        private static readonly int Show = Animator.StringToHash("show");
        private static readonly int Hide = Animator.StringToHash("hide");    
        public Color 
            normalColor, 
            speedColor, 
            forceColor, 
            duplicateColor, 
            phantomColor;
        public static PlayerController Instance { private set; get; }
        public ShootController shoot;
        public Animator 
            playerAnimator, 
            shootAnimation,
            knockOutAnimator;
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
        public Material playerColor;
        public GameObject model3D;
        public Sprite 
            shuriken, 
            pill, 
            multishot, 
            power, 
            speed, 
            phantom,
            empty;
        private PowerType 
            mPowerType;
        public bool
            powerIsOn,
            isHitByBoss;
        public int powerDuration;
        public PowerType PowerType
        {
            get => mPowerType;
            private set
            {
                switch(value)
                {
                    case PowerType.None:
                        PowerUp.Instance.icon.sprite = empty;
                        ChangePlayerColor(normalColor);
                        ChangeShootSpeedRate(1f);
                        DisableSecondaryShoot();
                        break; 
                    case PowerType.Duplicate:
                        PowerUp.Instance.icon.sprite = multishot;
                        ChangePlayerColor(duplicateColor);
                        ChangeShootSpeedRate(1f);
                        EnableSecondaryShoot();
                        break; 
                    case PowerType.Speed:
                        PowerUp.Instance.icon.sprite = speed;
                        ChangePlayerColor(speedColor);
                        ChangeShootSpeedRate(.5f);
                        DisableSecondaryShoot();
                        break;
                    case PowerType.Force:
                        PowerUp.Instance.icon.sprite = power;
                        ChangePlayerColor(forceColor);
                        ChangeShootSpeedRate(1f);
                        DisableSecondaryShoot();
                        break; 
                    case PowerType.Everything:
                        ChangePlayerColor(forceColor);
                        ChangeShootSpeedRate(.5f);
                        EnableSecondaryShoot(.5f);
                        break; 
                    case PowerType.Penetrating:
                        PowerUp.Instance.icon.sprite = phantom;
                        ChangePlayerColor(phantomColor);
                        ChangeShootSpeedRate(1f);
                        DisableSecondaryShoot();
                        break;
                }
                mPowerType = value;
            }
        }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            mScreenHeight = DataManagerUtility.GetScreenHeight();
        }

        private void Update()
        {
            Rotate();
        }
        private void Rotate(){
            mNewPos = transform.position;
            var media =  (mNewPos - mOldPos);
            mVelocity = media /Time.deltaTime;
            mOldPos = mNewPos;
            mNewPos = transform.position;
            model3D.transform.Rotate(Vector3.up, mVelocity.x * 5f);
            PlaySwimSound();
        }
        private void PlaySwimSound()
        {
            if(mVelocity.x <= 0 || !AudioManager.Instance) return;
        
            if (mVelocity.x > 90)
            {
                if(AudioManager.Instance.swimFast1Audio.isPlaying) return;
                AudioManager.Instance.swimFast1Audio.Play();
            } else if (mVelocity.x > 80)
            {
                if(AudioManager.Instance.swimFast2Audio.isPlaying) return;
                AudioManager.Instance.swimFast2Audio.Play();
            } else if (mVelocity.x > 60)
            {
                if(AudioManager.Instance.swimMedium1Audio.isPlaying) return;
                AudioManager.Instance.swimMedium1Audio.Play();
            } else if (mVelocity.x > 40)
            {
                if(AudioManager.Instance.swimMedium2Audio.isPlaying) return;
                AudioManager.Instance.swimMedium2Audio.Play();
            } else if (mVelocity.x > 20)
            {
                if(AudioManager.Instance.swimSoft1Audio.isPlaying) return;
                AudioManager.Instance.swimSoft1Audio.Play();
            } else
            {
                if(AudioManager.Instance.swimSoft2Audio.isPlaying) return;
                AudioManager.Instance.swimSoft2Audio.Play();
            }
        }

        private float mScreenHeight;
        private void FixedUpdate()
        {
            mMousePosition = Input.mousePosition;
            mMousePosition = Camera.main.ScreenToWorldPoint(mMousePosition);
            if(mMousePosition.y > mScreenHeight/2) return;
            if(Input.GetMouseButton(0)) transform.position = Vector2.Lerp(transform.position, new Vector3(mMousePosition.x, transform.position.y), 0.2f);
        }
        public List<GameObject> shoots = new List<GameObject>();
        private static readonly int Color = Shader.PropertyToID("_Color");

        public void Shoot()
        {
            if(GameController.Instance.gameState != GameState.Playing) return;
            if(!Input.GetMouseButton(0)) return;
            var shoot = Instantiate(this.shoot, trigger0.transform.position, Quaternion.identity);
            shoot.sprite.sprite = pill;
            shootAnimation.SetTrigger(Shoot1);
            if(AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
        }
        private void SecondaryShoot()
        {
            if(GameController.Instance.gameState != GameState.Playing) return;
            if(!Input.GetMouseButton(0)) return;
            var shoot1 = Instantiate(shoot, trigger1.transform.position, Quaternion.identity);
            shoot1.sprite.sprite = shuriken;
            var shoot2 = Instantiate(shoot, trigger2.transform.position, Quaternion.identity);
            shoot2.sprite.sprite = shuriken;
            if(AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(knockedOut) return;
            if (!other.gameObject.CompareTag("meteor") && !other.gameObject.CompareTag("boss")) return;
            if(AudioManager.Instance) AudioManager.Instance.punchAudio.Play();
            if(other.gameObject.CompareTag("meteor")) GameController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(other.GetComponent<SpermController>(), 0f, "", false));
            knockedOut = true;
            powerDuration = 0;
            knockOutAnimator.gameObject.SetActive(true);
            playerAnimator.SetTrigger(Intangible);
            StartCoroutine(ExitKnockOut());
            if(AudioManager.Instance) AudioManager.Instance.dizzyBirds.Play();

            /*
            if (GameController.Instance.gameState != GameState.Playing) return;
            
            if (AudioManager.Instance) AudioManager.Instance.playerCollisionAudio.Play();
            if (AudioManager.Instance) AudioManager.Instance.loseAudio.Play();
            
            if(isHitByBoss) return;
            if (other.gameObject.CompareTag("boss")) isHitByBoss = true; 
            GameController.Instance.SetGameState(GameController.Instance.CheckIfCanRetry() 
                ? GameState.Retry : GameState.Ended);*/
        }
        
        public bool knockedOut;
        private static readonly int Intangible = Animator.StringToHash("Intangible");

        private IEnumerator ExitKnockOut()
        {
            var knockOutDuration = 3f;
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
        private void DisableSecondaryShoot()
        {
            CancelInvoke(nameof(SecondaryShoot));
        }
        private void EnableSecondaryShoot(float modifier = 1f)
        {
            InvokeRepeating(nameof(SecondaryShoot), 0f, DataManagerUtility.GetShotSpeed(PlayerPrefs.GetInt("speed", 1)) * modifier);
        }
        private void ChangeShootSpeedRate(float modifier)
        { 
            CancelInvoke(nameof(Shoot));
            InvokeRepeating(nameof(Shoot), 0f, DataManagerUtility.GetShotSpeed(PlayerPrefs.GetInt("speed", 1))*modifier);
        }
        private IEnumerator EnablePower()
        {
            powerIsOn = true;
            while (powerDuration > 0)
            {
                yield return new WaitForSeconds(1f);
                powerDuration--;
            }
            PowerType = PowerType.None;
            powerIsOn = false;
        }
        public void GivePower(PowerType powerType)
        {
            if(PowerType != powerType)
            {
                PowerType = powerType;
                //StartCoroutine(PlayTrupmtAudio());
            }
            if(AudioManager.Instance) AudioManager.Instance.upgradeAudio.Play();
            powerDuration = 10;
            if (!powerIsOn)
            {
                StartCoroutine(EnablePower());
            }
        }

        public IEnumerator PlayTrupmtAudio()
        {
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.trumptAudio.Play();
        }
        
        public void ChangePlayerColor(Color color)
        {
            LeanTween.value(gameObject, playerColor.color, color, .5f).setOnUpdateColor((colorUpdate) =>
            {
                playerColor.SetColor(Color, colorUpdate);
            });
        }
    }
}