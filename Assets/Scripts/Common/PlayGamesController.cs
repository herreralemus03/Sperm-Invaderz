#if UNITY_ANDROID

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class PlayGamesController : MonoBehaviour
    {
        public GameObject loadingScreen;
        public Sprite gpgIconActive, gpgIconInactive;
        public Image gpgImage;
        public Button gpgButton, leaderBoardArcadeButton, leaderBoardEndlessButton;
        public static PlayGamesController Instance { private set; get; }
        public static PlayGamesPlatform platform;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (platform == null)
                {
                    var configuration = new PlayGamesClientConfiguration.Builder().Build();
                    PlayGamesPlatform.InitializeInstance(configuration);
                    platform = PlayGamesPlatform.Activate();
                }

                var logAutomatic = bool.Parse(PlayerPrefs.GetString("login_automatic", "true"));
                if (logAutomatic)
                {
                    Login();
                }
                ChangeColorIcon();
            }
            else
            {
                gpgButton.gameObject.SetActive(false);
            }
        }

        private void ChangeColorIcon()
        {
            var isAuth = PlayGamesPlatform.Instance.IsAuthenticated();
            gpgImage.sprite = isAuth ? gpgIconActive : gpgIconInactive;
            leaderBoardArcadeButton.gameObject.SetActive(isAuth);
            leaderBoardEndlessButton.gameObject.SetActive(isAuth);
        }

        public void Auth()
        {
            gpgButton.interactable = false;
            if (PlayGamesPlatform.Instance.IsAuthenticated())
                Logout();
            else
                Login();
        }

        private void Login()
        {
            loadingScreen.SetActive(true);
            Social.localUser.Authenticate((success) =>
            {
                if (success)
                {
                    Debug.Log("Logged in to Google Play Games Services");
                    PlayerPrefs.SetString("login_automatic", "true");
                    LoadEndlessHighScore();
                    LoadArcadeHighScore();
                }
                else
                {
                    Debug.Log("Failed to login to Google Play Games Services");
                }

                loadingScreen.SetActive(false);
                gpgButton.interactable = true;
                ChangeColorIcon();
            });
        }

        private void Logout()
        {
            PlayGamesPlatform.Instance.SignOut();
            ChangeColorIcon();
            PlayerPrefs.SetString("login_automatic", "false");
            gpgButton.interactable = true;
        }

        private void LoadAchievements()
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
            PlayGamesPlatform.Instance.LoadAchievements(achievements =>
            {
                if (achievements.Length > 0)
                {
                    foreach (var achievement in achievements)
                    {
                        PlayerPrefs.SetString(achievement.id, achievement.completed.ToString().ToLower());
                        Debug.Log(achievement.id);
                    }
                }
            });

            PlayerPrefs.SetString("achievements_loaded", "true");
        }

        public static void PostToLeaderBoardEndless(long score)
        {
            if (Application.platform != RuntimePlatform.Android) return;
            if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
            Social.ReportScore(score, GPGSIds.leaderboard_high_scores_endless, (success) =>
            {
                if (success)
                {
                    Debug.Log("Posted new score to leaderboard");
                }
                else
                {
                    Debug.Log("Unable to post new score to leaderboard");
                }
            });
        }

        public static void LoadEndlessHighScore()
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;

            PlayGamesPlatform.Instance.LoadScores(GPGSIds.leaderboard_high_scores_endless,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (data) =>
                {
                    PlayerPrefs.SetString("endless_high_score_amount", data.Scores[0].formattedValue);
                });
        }

        public static void LoadArcadeHighScore()
        {
            if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;

            PlayGamesPlatform.Instance.LoadScores(GPGSIds.leaderboard_high_scores_arcade,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                (data) =>
                {
                    PlayerPrefs.SetString("arcade_high_score_amount", data.Scores[0].formattedValue);
                });
        }
        public static void PostToLeaderBoardArcade(long score)
        {
            if (Application.platform != RuntimePlatform.Android) return;
            if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;
            Social.ReportScore(score, GPGSIds.leaderboard_high_scores_arcade, (success) =>
            {
                if (success)
                {
                    Debug.Log("Posted new score to leaderboard");
                }
                else
                {
                    Debug.Log("Unable to post new score to leaderboard");
                }
            });
        }

        public void ShowLeaderBoardUiArcade()
        {
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_scores_arcade);
            }
            else
            {
                Login();
            }
        }
        
        public void ShowLeaderBoardUiEndless()
        {
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_scores_endless);
            }
            else
            {
                Login();
            }
        }

        public void ShowAchievements()
        {
            if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.ShowAchievementsUI();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
#endif
