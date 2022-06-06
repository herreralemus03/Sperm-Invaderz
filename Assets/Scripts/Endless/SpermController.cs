using System;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Endless
{
    public class SpermController : MonoBehaviour
    {
        public int id;
        public Animator damageAnimation;
        public Rigidbody2D rb;
        public CapsuleCollider2D capsule;
        public SpermsType type;
        public bool isDestroyed, isEjected;
        public bool isInOvum;
        public bool tryToEnter;
        public AudioSource swimClip;

        public float Life { get; set; }
        [Range(1f, 10000f)] public float totalLife;

        private void Start()
        {
            totalLife = Life;
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.sortingOrder = -5;
            }
            rb.gravityScale = 0.05f + (GameController.Instance.level * 0.005f);
        }

        private void FixedUpdate()
        {
            CalculateVelocity();
            if (swimClip) swimClip.pitch = Math.Abs(mVelocity.y);
        }

        private float mAngle, mMoveSpeed;
        private Vector2 mMousePosition;
        private Vector3 mVelocity;
        private Vector3 mNewPos, mOldPos;

        private void CalculateVelocity()
        {
            var position = transform.position;
            mNewPos = position;
            var media = (mNewPos - mOldPos);
            mVelocity = media / Time.deltaTime;
            mOldPos = mNewPos;
            mNewPos = position;
        }

        private bool mFertilityIncremented;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("ovum")) return;
            if (mFertilityIncremented) return;
            mFertilityIncremented = true;
            if (GameController.Instance.gameState == GameState.Playing)
            {
                if (AudioManager.Instance) AudioManager.Instance.boingAudio.Play();
                if (bool.Parse(PlayerPrefs.GetString("vibrate", "true"))
                    && GameController.Instance.gameState == GameState.Playing) DataManagerUtility.Vibrate();
            }
            isInOvum = true;
            Invoke(nameof(Fertilize), 1f);
        }

        private void Fertilize()
        {
            capsule.isTrigger = true;
            if (type != SpermsType.FasterSperm) rb.velocity = Vector2.down;
            if (AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
        }
    }
}