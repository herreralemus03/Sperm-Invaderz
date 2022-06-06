using Common;
using UnityEngine;

namespace Invasion
{
    public class BossTrigger : MonoBehaviour
    {
        public Animator knockOutWithoutStars;

        public void SpawnSperm()
        {
            if(OvumController.Instance.Fertility <= 0f) return;
            if(AudioManager.Instance) AudioManager.Instance.spitAudio.Play();
            Instantiate(knockOutWithoutStars, transform.position, Quaternion.identity);
            var level = PlayerPrefs.GetInt("level", 10);
            StartCoroutine(SpermSpawnController.Instance.InstantiateSpermWithRandomLife(transform.position, level-1));
        }
    }
}