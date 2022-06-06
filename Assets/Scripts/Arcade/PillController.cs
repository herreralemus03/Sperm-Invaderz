using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Invasion;
using UnityEngine;

namespace Arcade
{
    public class PillController : MonoBehaviour
    {
        public Rigidbody2D rb;
        public GameObject smoke;
        public bool hit;
        private void Start()
        {
            rb.velocity = Vector3.right;
        }
        private void FixedUpdate()
        {
            if(hit) return;
            transform.Rotate(Vector3.back);
        }
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("shoot")) return;
            hit = true;
            rb.velocity = Vector2.zero;
            if(GameController.Instance.gameState != GameState.Playing) return;
            //StartCoroutine(DestroySpermRandomLy());
            DestroyPill();
        }

        public GameObject thunder;

        private void DestroyPill()
        {
            if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 1;
            if(AudioManager.Instance) AudioManager.Instance.scoreAudio.Play();
            var position = transform.position;
            LifeController.Instance.IncrementLife(position, 1);
            Instantiate(smoke, position, Quaternion.identity);
            if(gameObject) Destroy(gameObject);
        }

        private IEnumerator DestroySpermRandomLy()
        {
            var random = Random.Range(1, 3);
            var spermsToDestroy = new List<SpermController>();
            
            DestroyPill();
            
            for (var i = 0; i < random; i++)
            {
                if (SpermGroupMovement.Instance.sperms.Count < 5) break;
                var sperm = SpermGroupMovement.Instance.sperms.Where((spermT) => !spermT.selected).ToList()[
                    Random.Range(0, SpermGroupMovement.Instance.sperms.Count - 1)];
                sperm.selected = true;
                spermsToDestroy.Add(sperm);
            }
            if(gameObject) if(spermsToDestroy.Count <= 0) Destroy(gameObject);
            foreach (var sperm in spermsToDestroy)
            {
                mTarget = sperm;
                yield return new WaitForSeconds(0.11f);
                InstantiateThunder();
            }

            yield return null;
        }

        private SpermController mTarget;
        private void InstantiateThunder()
        {
            var instantiateThunder = Instantiate(this.thunder, transform.position, Quaternion.identity);
            LeanTween.move(instantiateThunder, mTarget.transform.position, 0.1f).setOnComplete(DestroyTarget);
        }

        private void DestroyTarget()
        {
            //GameController.Instance.StartCoroutine(GameController.Instance.DestroySperm(mTarget, 0f, ""));
            if(gameObject) Destroy(gameObject);
        }
    }
}