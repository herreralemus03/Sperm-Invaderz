using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Invasion
{
    public class SpermController : MonoBehaviour
    {
        public int id;
        public Animator damageAnimation;
        public Rigidbody2D rb;
        public CapsuleCollider2D capsule;
        public SpermsType type;
        public bool isDestroyed;
        public bool isInOvum;
        public bool tryToEnter;
        public AudioSource swimClip;
        public bool separated;
        public Animator animator;
        public bool special;
        public SpriteRenderer skin;
        public Sprite specialSprite, normalSprite;
        public float Life
        {
            get => mLife;
            set
            {
                mLife = value;
                if(lifeSlider) lifeSlider.slider.value = mLife;
            }
        }
        private float mLife;
        [Range(1f, 10000f)] public float totalLife;
        public HealthController lifeSlider;
        private void Start()
        {
            totalLife = mLife;
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.sortingOrder = -5;
            }
            switch (type)
            {
                case SpermsType.FasterSperm:
                    rb.gravityScale = 0.2f;
                    rb.drag = .5f;
                    break;
                case SpermsType.Beast:
                    rb.drag = 0f;
                    rb.gravityScale = 0f;
                    rb.velocity = Vector2.down * .4f;
                    break;
                default:
                    rb.drag = 0f;
                    rb.gravityScale = 0f;
                    rb.velocity = Vector2.down * .8f;
                    break;
            }
            if (special)
            {
                skin.sprite = specialSprite;
                animator.SetTrigger(Special);
            }
        }
        private void FixedUpdate()
        {
            CalculateVelocity();
            if (swimClip) swimClip.pitch = Math.Abs(mVelocity.y);
            if (lifeSlider) lifeSlider.transform.position = transform.position + (Vector3.down * 0.1f);
            //lifeSlider.gameObject.SetActive(GameController.Instance.gameState == GameState.Playing);
        }

        private float mAngle, mMoveSpeed;
        private Vector2 mMousePosition;
        private Vector3 mVelocity;
        private Vector3 mNewPos, mOldPos;

        private void CalculateVelocity()
        {
            mNewPos = transform.position;
            var media = (mNewPos - mOldPos);
            mVelocity = media / Time.deltaTime;
            mOldPos = mNewPos;
            mNewPos = transform.position;
        }

        private bool mFertilityIncremented;
        private static readonly int Special = Animator.StringToHash("Special");

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("ovum")) return;
            if (!mFertilityIncremented)
            {
                mFertilityIncremented = true;
                if (GameController.Instance.gameState == GameState.Playing)
                {
                    if (AudioManager.Instance) AudioManager.Instance.boingAudio.Play();
                    if (bool.Parse(PlayerPrefs.GetString("vibrate", "true"))
                        && GameController.Instance.gameState == GameState.Playing) DataManagerUtility.Vibrate();
                }

                if (OvumController.Instance.Fertility <= 98)
                {
                    OvumController.Instance.Fertility +=
                        Mathf.RoundToInt((200f - PlayerPrefs.GetInt("level", 1)) / 78f) + 2;
                }

                isInOvum = true;
                var canFertilize = DataManagerUtility.CanFertilize(OvumController.Instance.Fertility);
                if (canFertilize)
                {
                    Invoke(nameof(Fertilize), 1f);
                    return;
                }

                tryToEnter = true;
                SpermSpawnController.Instance.StartCoroutine(
                    SpermSpawnController.Instance.DestroySperm(this, 4f, "sperm controller: on collision enter 2d",
                        false));
            }
        }
        private void Fertilize()
        {
            if (OvumController.Instance.alreadyFertilized)
            {
                //SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(this, 4f));
                return;
            }

            OvumController.Instance.alreadyFertilized = true;
            capsule.isTrigger = true;
            if (type != SpermsType.FasterSperm) rb.velocity = Vector2.down;
            if (AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
        }
        private void OnBecameInvisible()
        {
            if (isInOvum) return;
            if (SpermSpawnController.Instance && SpermSpawnController.Instance.arcade) return;
            //Destroy(gameObject);
        }
    }
}