using Common;
using TMPro;
using UnityEngine;

namespace Endless
{
    public class ScoreController : MonoBehaviour
    {
        public static ScoreController Instance { private set; get; }
        public ScoreText scoreText;
        public TextMeshProUGUI textMesh;
        public long scoreAmount;
        public Animator animator;
        public Canvas parent;
        private static readonly int Increase = Animator.StringToHash("Increase");
        private long mHighScore;
        private void Awake()
        {
            if (Instance == null) Instance = this;
#if UNITY_ANDROID
            PlayGamesController.LoadEndlessHighScore();
#endif
        }

        private void Start()
        {
            mHighScore = long.Parse(PlayerPrefs.GetString("endless_high_score_amount", "0"));
        }

        private long ScoreAmount
        {
            set
            {
                scoreAmount += value;
                Invoke(nameof(IncreaseScoreAmountOnUiInfiniteMode), 1f);
            }
        }

        private void IncreaseScoreAmountOnUiInfiniteMode()
        {
            if (AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
            animator.SetTrigger(Increase);
            textMesh.text = DataManagerUtility.FormatAmount(scoreAmount);
            if (scoreAmount > mHighScore)
            {
                ScreensInGameManager.Instance.highScore.text = $"{scoreAmount}";
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void InstantiateScore(Vector3 scorePosition, int amount)
        {
            scoreText.movePoint = transform.position;
            var scoreStr = Instantiate(scoreText, scorePosition, Quaternion.identity, parent.transform);
            ScoreAmount = amount;
            scoreStr.textMesh.text = $"+{amount}";
        }

    }
}