using GooglePlayGames;
using UnityEngine;

namespace Common
{
    public class LeaderBoardPlugin : MonoBehaviour
    {
        public void ShowLeaderBoardUiArcade()
        {
            #if UNITY_ANDROID
            if(PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_scores_arcade);
            }
            else
            {
                Login();
            }
            #endif
        }
        public void ShowLeaderBoardUiEndless()
        {
            #if UNITY_ANDROID
            if(PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_scores_endless);
            }
            else
            {
                Login();
            }
            
            #endif
        }
    
        private void Login()
        {
            Social.localUser.Authenticate((success) =>
            {
                if (success)
                {
                    Debug.Log("Logged in to Google Play Games Services");
                    PlayerPrefs.SetString("login_automatic", "true");
                    if(Arcade.ScoreController.Instance && Arcade.ScoreController.Instance.tempScore > 0L)
                    {
                        if(Arcade.GameController.Instance.gameState == GameState.Ended)
                        {
                            #if UNITY_ANDROID
                            PlayGamesController.PostToLeaderBoardArcade(Arcade.ScoreController.Instance.tempScore);
                            #endif
                        }
                        if(Arcade.ScreensInGameManager.Instance) Arcade.ScreensInGameManager.Instance.LoadHighScores();
                    }
                    if(Endless.ScoreController.Instance && Arcade.ScoreController.Instance.tempScore > 0L)
                    {
                        if(Endless.GameController.Instance.gameState == GameState.Ended)
                        {
                            #if UNITY_ANDROID
                            PlayGamesController.PostToLeaderBoardEndless(Endless.ScoreController.Instance.scoreAmount);
                            #endif
                        }
                        if(Endless.ScreensInGameManager.Instance) Endless.ScreensInGameManager.Instance.LoadHighScores();
                    }
                }
                else
                {
                    Debug.Log("Failed to login to Google Play Games Services");
                }
            });
        }
    }
}