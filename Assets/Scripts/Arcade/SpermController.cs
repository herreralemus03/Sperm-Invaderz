using Common;
using UnityEngine;

namespace Arcade
{
    public class SpermController : MonoBehaviour
    {
        public int id;
        public Animator damageAnimation;
        public Rigidbody2D rb;
        public CapsuleCollider2D capsule;
        public SpermsType type;
        public bool separated, selected;
        public float Life { get; set; }
        [Range(1f, 10000f)] 
        public float totalLife;  
        private void Start()
        {
            totalLife = Life;
            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.sortingOrder = -5;
            }
        }

        private bool mFertilityIncremented;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("ovum")) return;
            if (mFertilityIncremented) return;
            mFertilityIncremented = true;
            if (GameController.Instance.gameState == GameState.Playing)
            {
                if(AudioManager.Instance) AudioManager.Instance.boingAudio.Play();
                if(bool.Parse(PlayerPrefs.GetString("vibrate", "true")) 
                   && GameController.Instance.gameState == GameState.Playing) DataManagerUtility.Vibrate();
            }
            Invoke(nameof(Fertilize), 1f);
        }


        private void Fertilize()
        {
            capsule.isTrigger = true;
            if(type != SpermsType.FasterSperm) rb.velocity = Vector2.down;
            if(AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
        }
    
        private void OnDestroy()
        {
            if(SpermGroupMovement.Instance)
            {
                SpermGroupMovement.Instance.UpdateSpermList(id, true);
            }
        }
    }
}