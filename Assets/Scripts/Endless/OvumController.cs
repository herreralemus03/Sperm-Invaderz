using UnityEngine;
using Common;
using TMPro;
namespace Endless
{
    public class OvumController : MonoBehaviour
    {
        private bool mLevelCleared;
        public bool isInmune;
        public static OvumController Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameController.Instance.gameState != GameState.Playing || isInmune) return;
            if (!other.gameObject.CompareTag("meteor")) return;
            isInmune = true;
            if (AudioManager.Instance) AudioManager.Instance.loseAudio.Play();
            GameController.Instance.SetGameState(GameController.Instance.CheckIfCanRetry()
                ? GameState.Retry
                : GameState.Ended);
        }
    }
}