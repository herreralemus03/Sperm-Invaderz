using UnityEngine;
using System.Collections;
using Common;
using GooglePlayGames;

namespace Arcade
{
    public class GameController : MonoBehaviour
    {
        public GameState gameState;

        /*public GameObject
            scoreScreen,
            retryScreen,
            uiElements,
            pauseScreen,
            clearLevelScreen,
            highScore;*/

        /*public Button pauseButton;*/

        /*public TextMeshProUGUI
            countDownText,
            levelText,
            highScoreAmount;*/

        /*public Toggle vibrationToggle;

        public Slider
            musicSliderVolume,
            sfxSliderVolume;*/

        public AudioSource backgroundMusic;
        /*public int countDown;*/

        public bool
            levelCleared,
            scorePosted;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            #if UNITY_ANDROID
            if(PlayGamesPlatform.Instance.IsAuthenticated())
            {
                ScreensInGameManager.Instance.highScoreLabel.SetActive(true);
                ScreensInGameManager.Instance.highScore.text = PlayerPrefs.GetString("arcade_high_score_amount", "0");
            }
            else
            {
                ScreensInGameManager.Instance.highScoreLabel.SetActive(false);
            }
            #endif

            LoadDefaultVolume();
            ScreensInGameManager.Instance.pauseButton.interactable = true;
            ScreensInGameManager.Instance.levelLabel.text = $"{ScoreArcadeMode.Instance.level}";
            ScoreController.Instance.textMesh.text = $"{ScoreArcadeMode.Instance.score}";
            PlayerController.Instance.InvokeRepeating(nameof(PlayerController.Instance.Shoot), 2f, 0.5f);
        }

        public void LoadDefaultVolume()
        {
            PreferencesManager.Instance.music.SetFloat("music_volume", PlayerPrefs.GetFloat("music_volume", 0f));
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", PlayerPrefs.GetFloat("sfx_volume", 0f));
        }
        
        private void MuteSounds()
        {
            var storageVolume = PreferencesManager.Instance.GetMusicVolume();
            var modifiedVolume = DataManagerUtility.GetModifiedVolumeForPause(storageVolume);
            PreferencesManager.Instance.music.SetFloat("music_volume", modifiedVolume);
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", -80f);
        }

        public void LoadPreferences()
        {
            ScreensInGameManager.Instance.vibrationToggle.isOn = PreferencesManager.Instance.GetVibration();
            ScreensInGameManager.Instance.musicSlider.value = PreferencesManager.Instance.GetMusicVolume();
            ScreensInGameManager.Instance.sfxSlider.value = PreferencesManager.Instance.GetSfxVolume();
        }

        private IEnumerator StopMusic()
        {
            while (backgroundMusic.volume > 0)
            {
                backgroundMusic.volume -= 0.1f;
                yield return null;
            }

            backgroundMusic.Stop();
            MuteSounds();
        }
        public static GameController Instance { private set; get; }

        public void PauseGame()
        {
            SetGameState(GameState.Paused);
            LoadPreferences();
        }

        public void ResumeGame() => StartCoroutine(ResumeGameCoroutine());

        private IEnumerator ResumeGameCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            SetGameState(GameState.Playing);
            yield return null;
        }

        private bool mShowingAd;

        public void PauseGameOnShowAd()
        {
            mShowingAd = true;
            Time.timeScale = 0;
            PreferencesManager.Instance.music.SetFloat("music_volume", -80f);
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", -80f);
            PreferencesManager.Instance.ui.SetFloat("ui_volume", -80f);
        }
        private static void ResumeGameOnCloseAd()
        {
            Time.timeScale = 1;
            PreferencesManager.Instance.music.SetFloat("music_volume", PreferencesManager.Instance.GetMusicVolume());
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", PreferencesManager.Instance.GetSfxVolume());
            PreferencesManager.Instance.ui.SetFloat("ui_volume", PreferencesManager.Instance.GetUiVolume());
        }

        public void SetGameState(GameState state)
        {
            if(gameState == GameState.Cleared || gameState == GameState.Ended) return;
            gameState = state;
            switch (gameState)
            {
                case GameState.Cleared:
                    if (levelCleared) return;
                    MuteSounds();
                    levelCleared = true;
                    StartCoroutine(StopMusic());
                    Invoke(nameof(ShowClearGameScreen), 0f);
                    break;
                case GameState.Playing:
                    LoadDefaultVolume();
                    ScreensInGameManager.Instance.ShowHudElements();
                    Time.timeScale = 1;
                    break;
                case GameState.Ended:
                    if (scorePosted) return;
                    PostToLeaderBoard();
                    ScreensInGameManager.Instance.ShowGameOverScreenElements();
                    PlayGameOverAudio();
                    StartCoroutine(StopMusic());
                    Time.timeScale = 1;
                    break;
                case GameState.Paused:
                    MuteSounds();
                    ScreensInGameManager.Instance.ShowPauseScreen();
                    Time.timeScale = 0;
                    break;
            }
        }

        private void PlayGameOverAudio()
        {
            if(AudioManager.Instance) AudioManager.Instance.gameOverAudio.Play();
        }

        private void PostToLeaderBoard()
        {
            #if UNITY_ANDROID
            PlayGamesController.PostToLeaderBoardArcade(ScoreArcadeMode.Instance.score);
            #endif
            scorePosted = true;
        }

        public void ResetValues()
        {
            ScoreController.Instance.tempScore = ScoreArcadeMode.Instance.score;
            ScoreArcadeMode.Instance.score = 0L;
            ScoreArcadeMode.Instance.level = 1;
            ScoreArcadeMode.Instance.attempts = 1;
            ScoreArcadeMode.Instance.playerLife = 2;
#if UNITY_ANDROID
            PlayGamesController.LoadArcadeHighScore();
#endif
        }
        public void PlayPopAudio()
        {
            if (AudioManager.Instance) AudioManager.Instance.clickAudio.Play();
        }

        public void ShowClearGameScreen()
        {
            if(AudioManager.Instance) AudioManager.Instance.clearAudio.Play();
            ScoreController.Instance.tempScore = ScoreArcadeMode.Instance.score;
            ScreensInGameManager.Instance.ShowClearScreenElements();
        }

        public void IncrementLevel() => ScoreArcadeMode.Instance.level++;
        public GameObject lifeDialogReward;
        public GameObject rewardParent;
        public IEnumerator IncrementLife()
        {
            ScoreArcadeMode.Instance.playerLife++;
            Instantiate(lifeDialogReward, rewardParent.transform.position, Quaternion.identity, rewardParent.transform);
            if(AudioManager.Instance) AudioManager.Instance.boingAudio2.Play();
            yield return null;
        }

        public void ShowAdToIncrementLife()
        {
            ScreensInGameManager.Instance.diuImage.SetActive(true);
            if(UnityAdsManager.Instance && UnityAdsManager.Instance.canRetry && ScoreArcadeMode.Instance.attempts > 0)
                UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Retry, GameMode.Arcade);
        }
        public GameObject knockOutAnimationWithoutStars, knockOutAnimation;
        public IEnumerator DestroySperm(SpermController objectToDestroy, float time, string location, bool destroyedByPlayer = false, bool spawnOther = false)
        {
            yield return new WaitForSeconds(time);
            if (objectToDestroy) Instantiate(spawnOther ? knockOutAnimationWithoutStars : knockOutAnimation, objectToDestroy.gameObject.transform.position, Quaternion.identity);
            if(!spawnOther && AudioManager.Instance) AudioManager.Instance.explosionAudio.Play();
            if(spawnOther && AudioManager.Instance) AudioManager.Instance.spawnAudio.Play();
            if (objectToDestroy) Destroy(objectToDestroy.gameObject);
        }
        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnGUI()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameState == GameState.Playing)
                {
                    PauseGame();
                }
            }
        }
    }
}