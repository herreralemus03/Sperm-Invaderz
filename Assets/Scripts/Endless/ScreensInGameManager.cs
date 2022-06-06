using System;
using System.Collections;
using System.Linq;
using Common;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Endless
{
    public class ScreensInGameManager : MonoBehaviour
    {
        [Header("Animator")] 
        public Animator screenGroupAnimator;

        [Header("HUD")] 
        public GameObject highScoreLabel;
        public TextMeshProUGUI highScore;
        public Button pauseButton;
        public ScoreController scoreController;
        [Header("Retry elements")]
        public GameObject heroAnimator;
        public Button retryButton;
        public Button cancelButton;
        public TextMeshProUGUI description;
        public TextMeshProUGUI countDownText;
        [Header("Fail level")] 
        public Button reloadLevelButton;
        [Header("Pause Screen")] 
        public Button resumeGameButton;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle vibrationToggle;
        [Header("Common")] 
        public GameObject dialogBox;
        public GameObject scoreParent;
        public TextMeshProUGUI score;
        public TextMeshProUGUI title;
        public GameObject panel;
        public TextMeshProUGUI titlePanel;
        public GameObject titlePanelParent;
        public Button homeButton;
        [Header("Common level finish")] 
        public GameObject highScoresGroup;
        public TextMeshProUGUI firstPlaceUsername;
        public TextMeshProUGUI secondPlaceUsername;
        public TextMeshProUGUI thirdPlaceUsername;
        public TextMeshProUGUI firstPlaceScore;
        public TextMeshProUGUI secondPlaceScore;
        public TextMeshProUGUI thirdPlaceScore;
        
        private static readonly int Hide = Animator.StringToHash("Hide");
        private static readonly int Show = Animator.StringToHash("Show");

        public static ScreensInGameManager Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        private void HudElements(bool show)
        {
            pauseButton.gameObject.SetActive(show);
            if(show)
            {
                #if UNITY_ANDROID
                if (PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    highScoreLabel.gameObject.SetActive(show);
                }
                else
                {
                    highScore.text = PlayerPrefs.GetString("endless_high_score_amount", "0");
                }
                #endif
            }
            scoreController.gameObject.SetActive(show);
        }

        private void DialogBox(bool show)
        {
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            title.gameObject.SetActive(show);
            panel.SetActive(show);
            dialogBox.SetActive(show);
            scoreParent.SetActive(show);
            score.gameObject.SetActive(show);
            if (show) score.text = $"{ScoreController.Instance.scoreAmount}";
            if(GameController.Instance.gameState != GameState.Retry) homeButton.gameObject.SetActive(show);
        }
        private void SetHeaders(string titleStr, string subtitleStr)
        {
            titlePanel.text = subtitleStr;
            title.text = titleStr;
        }
        private void PauseScreenElements(bool show)
        {
            DialogBox(show);
            if(show) SetHeaders("PAUSED", "SETTINGS");
            resumeGameButton.gameObject.SetActive(show);
            musicSlider.gameObject.SetActive(show);
            sfxSlider.gameObject.SetActive(show);
            vibrationToggle.gameObject.SetActive(show);
        }
        private void GameOverScreenElements(bool show)
        {        
            DialogBox(show);
            if(show) SetHeaders("GAME\nOVER", "BEST SCORES");
            reloadLevelButton.gameObject.SetActive(show);
            if(show) ActivateLeaderBoardButton();
            homeButton.gameObject.SetActive(show);
        }

        private void RetryScreenElements(bool show)
        {
            DialogBox(show);
            if(show) SetHeaders("RETRY", "SUPER BPC");
            retryButton.gameObject.SetActive(show);
            cancelButton.gameObject.SetActive(show);
            description.gameObject.SetActive(show);
            countDownText.gameObject.SetActive(show);
            heroAnimator.gameObject.SetActive(show);
        }
        
        public Button leaderBoardButton;
        public Image leaderBoardImage;
        public Sprite leaderBoardSprite, gpgSprite;
        public GameObject loginLabel;
        public GameObject loadingLabel;
        private void ActivateLeaderBoardButton()
        {
            #if UNITY_ANDROID
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                LoadHighScores();
            }
            else
            {                
                leaderBoardImage.sprite = gpgSprite;
                loginLabel.SetActive(true);
                highScoresGroup.SetActive(false);
            }
            #endif
            
            leaderBoardButton.gameObject.SetActive(true);
        }
        
        public void ShowPauseScreen()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> RetryScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(false),
                ()=> PauseScreenElements(true)
            ));
        }

        private IEnumerator MakeScreenTransition(
            Action hideScreen1,
            Action hideScreen2,
            Action hideScreen3,
            Action showScreen)
        {
            screenGroupAnimator.SetTrigger(Hide);
            yield return new WaitForSecondsRealtime(0.5f);
            hideScreen1();
            hideScreen2();
            hideScreen3();
            showScreen();
        }
        public void ShowRetryScreenElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(false),
                ()=> RetryScreenElements(true)
            ));
        }
        public void ShowGameOverScreenElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> RetryScreenElements(false),
                ()=> HudElements(false),
                ()=> GameOverScreenElements(true)
            ));
        }
        public void ShowHudElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> RetryScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(true)
            ));
        }

        public void LoadHighScores(LeaderboardTimeSpan timeSpan = LeaderboardTimeSpan.AllTime)
        {
            leaderBoardImage.sprite = leaderBoardSprite;
            loginLabel.SetActive(false);
            highScoresGroup.SetActive(false);
            loadingLabel.SetActive(true);
            
            #if UNITY_ANDROID
                        PlayGamesPlatform.Instance.LoadScores(GPGSIds.leaderboard_high_scores_endless,
                            LeaderboardStart.PlayerCentered,
                            3,
                            LeaderboardCollection.Public,
                            timeSpan,
                            (data) =>
                            {
                                var scores = data.Scores;
                                var userIds = scores.Select(userScore => userScore.userID).ToList();

                                mFirstPlaceUserId = userIds[0];
                                mFirstPlaceUserScore = scores[0].formattedValue;
                                mSecondPlaceUserId = userIds[1];
                                mSecondPlaceUserScore = scores[1].formattedValue;
                                mThirdPlaceUserId = userIds[2];
                                mThirdPlaceUserScore = scores[2].formattedValue;
                                
                                if(timeSpan == LeaderboardTimeSpan.AllTime) PlayerPrefs.SetString("endless_high_score_amount", mFirstPlaceUserScore);
                                Social.LoadUsers(userIds.ToArray(), (profiles) =>
                                {
                                    mFirstPlaceUserName = profiles.First((profile) => mFirstPlaceUserId == profile.id).userName;
                                    mSecondPlaceUserName = profiles.First((profile) => mSecondPlaceUserId == profile.id).userName;
                                    mThirdPlaceUserName = profiles.First((profile) => mThirdPlaceUserId == profile.id).userName;
                                    DisplayLeaderBoardEntries();
                                });
                            });
            #endif
        }

        private string mFirstPlaceUserName;
        private string mFirstPlaceUserId;
        private string mFirstPlaceUserScore;
        private string mSecondPlaceUserName;
        private string mSecondPlaceUserId;
        private string mSecondPlaceUserScore;
        private string mThirdPlaceUserName;
        private string mThirdPlaceUserId;        
        private string mThirdPlaceUserScore;
        private void DisplayLeaderBoardEntries()
        {
            firstPlaceScore.text = mFirstPlaceUserScore ?? "-";
            firstPlaceUsername.text = mFirstPlaceUserName ?? "-";
            secondPlaceScore.text = mSecondPlaceUserScore ?? "-";
            secondPlaceUsername.text = mSecondPlaceUserName ?? "-";
            thirdPlaceScore.text = mThirdPlaceUserScore ?? "-";
            thirdPlaceUsername.text = mThirdPlaceUserName ?? "-";
            loadingLabel.SetActive(false);
            highScoresGroup.SetActive(true);
        }
    }
}