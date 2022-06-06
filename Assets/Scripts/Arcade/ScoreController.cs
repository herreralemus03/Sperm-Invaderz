using Common;
using TMPro;
using UnityEngine;

namespace Arcade
{
    public class ScoreController : MonoBehaviour
    {
        public static ScoreController Instance { private set; get; }
        public ScoreText scoreText;
        public TextMeshProUGUI textMesh;
        public Animator animator;
        private long mScore;
        private long mHighScore;
        public long tempScore;
        public long ScoreAmount
        {
            get => mScore;
            set
            {
                ScoreArcadeMode.Instance.score += value;
                Invoke(nameof(IncreaseScoreAmountOnUiArcadeMode), 1f);
            }
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            mHighScore = long.Parse(PlayerPrefs.GetString("arcade_high_score_amount", "0"));
        }

        private void IncreaseScoreAmountOnUiArcadeMode()
        {
            if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
            animator.SetTrigger(Increase);
            textMesh.text = DataManagerUtility.FormatAmount(ScoreArcadeMode.Instance.score);
            if (ScoreArcadeMode.Instance.score > mHighScore)
            {
                ScreensInGameManager.Instance.highScore.text = $"{ScoreArcadeMode.Instance.score}";
            }
        }

        public void InstantiateScore(Vector3 scorePosition, int amount)
        {
            scoreText.movePoint = transform.position;
            var scoreStr = Instantiate(scoreText, scorePosition, Quaternion.identity);
            var multiplier = PlayerController.Instance.spermsDestroyedConsecutive;
            
            if(PlayerController.Instance.spermsDestroyedConsecutive > 1)
            {
                amount *= multiplier;
                InstantiateScoreTextBonus(multiplier, scorePosition);
            }
            
            scoreStr.textMesh.text = $"+{amount}";
            ScoreAmount += amount;
        }

        public TextMeshProUGUI bonusTextMesh;
        public Canvas parent;
        private static readonly int Increase = Animator.StringToHash("Increase");

        private void InstantiateScoreTextBonus(int multiplier, Vector3 position)
        {
            switch (multiplier)
            {
                case 2:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 1f;
                    bonusTextMesh.text = "GOOD";
                    break;
                case 3:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.9f;
                    bonusTextMesh.text = "NICE";
                    break;
                case 4:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.8f;
                    bonusTextMesh.text = "GREAT";
                    break;
                case 5:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.7f;
                    bonusTextMesh.text = "AWESOME";
                    break;
                case 6:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.6f;
                    bonusTextMesh.text = "AMAZING";
                    break;
                case 7:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.5f;
                    bonusTextMesh.text = "WONDERFUL";
                    break;
                case 8:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.4f;
                    bonusTextMesh.text = "FANTASTIC";
                    break;
                default:
                    if(AudioManager.Instance) AudioManager.Instance.scoreAudio.pitch = 0.4f;
                    bonusTextMesh.text = "FANTASTIC";
                    break;
            }

            if(AudioManager.Instance) AudioManager.Instance.scoreAudio.Play();
        
            Instantiate(bonusTextMesh, position, Quaternion.identity, parent.transform);
        }
    }
}