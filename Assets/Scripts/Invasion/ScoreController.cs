using System;
using Common;
using TMPro;
using UnityEngine;

namespace Invasion
{
    public class ScoreController : MonoBehaviour
    {
        private static readonly int Increase = Animator.StringToHash("Increase");
        public static ScoreController Instance { private set; get; }
        public ScoreText scoreText;
        public TextMeshProUGUI scoreLabel, scoreLabel2;
        public Animator animator;
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            scoreLabel.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            ScreensInGameManager.Instance.credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
        }

        public long tempCredits;
        private long ScoreAmount
        {
            set
            {
                if(GameController.Instance.gameState == GameState.Playing) tempCredits += value;
                DataManagerUtility.AddCreditsAmount(Convert.ToInt64(value));
                Invoke(nameof(IncreaseScoreAmountOnUi), 1f);
            }
        }

        private void IncreaseScoreAmountOnUi()
        {
            if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
            animator.SetTrigger(Increase);
            scoreLabel.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void InstantiateScoreText(Vector3 scorePosition, int amount)
        {
            scoreText.movePoint = transform.position;
            var scoreStr = Instantiate(scoreText, scorePosition, Quaternion.identity);
            scoreStr.textMesh.text = $"+{amount}";
            ScoreAmount = amount;
        }

    }
}