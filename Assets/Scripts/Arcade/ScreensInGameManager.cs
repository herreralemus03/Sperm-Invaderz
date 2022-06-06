using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Arcade
{
    public class ScreensInGameManager : MonoBehaviour
    {
        [Header("Animator")] 
        public Animator screenGroupAnimator;

        [Header("HUD")] 
        public LifeController lifeLabel;
        public GameObject highScoreLabel;
        public TextMeshProUGUI highScore;
        public Button pauseButton;
        public ScoreController scoreController;
        public GameObject levelParent;
        public TextMeshProUGUI levelLabel;
        [Header("Clear level")] 
        public GameObject diuImage;
        public Button nextLevelButton;
        public GameObject heroAnimator;
        public TextMeshProUGUI lifeLabel2;
        [Header("Fail level")] 
        public Button reloadLevelButton;
        [Header("Pause Screen")] 
        public Button resumeGameButton;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle vibrationToggle;
        [Header("Common")] 
        public GameObject lifeLabelParent2;
        public Animator lifeLabelAnimator;
        public Animator lifeLabelAnimator2;
        public GameObject dialogBox;
        public GameObject scoreParent;
        public TextMeshProUGUI score;
        public TextMeshProUGUI title;
        public GameObject panel;
        public TextMeshProUGUI titlePanel;
        public GameObject titlePanelParent;
        public Button homeButton;
        [Header("Common level finish")] 
        public TextMeshProUGUI score2;
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
            lifeLabel.gameObject.SetActive(show);
            if(show)
            {
                #if UNITY_ANDROID
                if (PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    highScore.text = PlayerPrefs.GetString("arcade_high_score_amount", "0");
                }
                #endif
            }
                
            highScoreLabel.gameObject.SetActive(show);
            scoreController.gameObject.SetActive(show);
            levelParent.gameObject.SetActive(show);
        }

        private void DialogBox(bool show)
        {
            lifeLabelParent2.SetActive(show);
            lifeLabel2.text = $"{ScoreArcadeMode.Instance.playerLife}";
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            title.gameObject.SetActive(show);
            panel.SetActive(show);
            dialogBox.SetActive(show);
            scoreParent.SetActive(show);
            score.gameObject.SetActive(show);
            if (show) score.text = $"{ScoreArcadeMode.Instance.score}";
            if (show) score2.text = $"{ScoreArcadeMode.Instance.score}";
            homeButton.gameObject.SetActive(show);
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
        private void ClearScreenElements(bool show)
        { 
            DialogBox(show);
            if(show) SetHeaders($"LEVEL\n{ScoreArcadeMode.Instance.level}\nCLEAR", "");
            nextLevelButton.gameObject.SetActive(show);
            homeButton.gameObject.SetActive(show);
            if ((ScoreArcadeMode.Instance.attempts > 0 && ScoreArcadeMode.Instance.playerLife < 3) || !show)
            {
                heroAnimator.gameObject.SetActive(show);
            }
            else
            {
                diuImage.SetActive(show);
            }
            titlePanelParent.SetActive(false);
            //if(show) ActivateLeaderBoardButton();
            GameController.Instance.IncrementLevel();
        }
        private void GameOverScreenElements(bool show)
        {        
            DialogBox(show);
            if(show) SetHeaders("GAME\nOVER", "BEST SCORES");
            lifeLabelParent2.SetActive(false);
            reloadLevelButton.gameObject.SetActive(show);
            if(show) ActivateLeaderBoardButton();
            homeButton.gameObject.SetActive(show);
            if(show) GameController.Instance.ResetValues();
        }
        
        public Button leaderBoardButton;
        public Image leaderBoardImage;
        public Sprite leaderBoardSprite, gpgSprite;
        public GameObject loginLabel;
        public GameObject loadingLabel;
        public void ActivateLeaderBoardButton()
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
            #else 
                leaderBoardImage.sprite = gpgSprite;
                loginLabel.SetActive(true);
                highScoresGroup.SetActive(false);
            #endif
            leaderBoardButton.gameObject.SetActive(true);
        }
        
        public void ShowPauseScreen()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> ClearScreenElements(false),
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
        public void ShowClearScreenElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(false),
                ()=> ClearScreenElements(true)
            ));
        }
        public void ShowGameOverScreenElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> ClearScreenElements(false),
                ()=> HudElements(false),
                ()=> GameOverScreenElements(true)
            ));
        }
        public void ShowHudElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> ClearScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(true)
            ));
        }

        public void LoadHighScores(LeaderboardTimeSpan timeSpan = LeaderboardTimeSpan.AllTime)
        {
            leaderBoardImage.sprite = leaderBoardSprite;
            highScoresGroup.SetActive(false);
            loginLabel.SetActive(false);
            loadingLabel.SetActive(true);
            #if UNITY_ANDROID 
            PlayGamesPlatform.Instance.LoadScores(GPGSIds.leaderboard_high_scores_arcade,
                LeaderboardStart.PlayerCentered,
                3,
                LeaderboardCollection.Public,
                timeSpan,
                (data) => {
                    var scores = data.Scores;
                    var userIds = scores.Select(userScore => userScore.userID).ToList();

                    mFirstPlaceUserId = userIds[0];
                    mFirstPlaceUserScore = scores[0].formattedValue;
                    mSecondPlaceUserId = userIds[1];
                    mSecondPlaceUserScore = scores[1].formattedValue;
                    mThirdPlaceUserId = userIds[2];
                    mThirdPlaceUserScore = scores[2].formattedValue;
                    if(timeSpan == LeaderboardTimeSpan.AllTime) PlayerPrefs.SetString("arcade_high_score_amount", mFirstPlaceUserScore);
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