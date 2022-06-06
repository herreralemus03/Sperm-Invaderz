using System;
using System.Collections;
using System.Linq;
using Common;
using GooglePlayGames;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Endless
{
    public class GameController : MonoBehaviour
    {
        public GameState gameState;
        
        /*public GameObject
            highScore,
            scoreScreen, 
            retryScreen, 
            uiElements, 
            pauseScreen;*/
        /*public Button pauseButton;*/
        /*public TextMeshProUGUI countDownText;/*, highScoreAmount;
        public Toggle vibrationToggle;
        public Slider
            musicSliderVolume,
            sfxSliderVolume;*/
        public AudioSource backgroundMusic;
        public int 
            level,
            countDown, 
            attempts = 1;
        private bool 
            mShowingAd,
            mLevelCleared, 
            mScorePosted;
        [HideInInspector] 
        public bool cancelCountDown;
        private IEnumerator mStartCountDown;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            mStartCountDown = UpdateCountDown();
            
        }

        private void Start()
        {
            attempts = 1;
            LoadDefaultVolume();
            #if UNITY_ANDROID
                        if(PlayGamesPlatform.Instance.IsAuthenticated())
                        {
                            ScreensInGameManager.Instance.highScoreLabel.SetActive(true);
                            ScreensInGameManager.Instance.highScore.text = PlayerPrefs.GetString("endless_high_score_amount", "0");
                        }
                        else
                        {
                            ScreensInGameManager.Instance.highScoreLabel.SetActive(false);
                        }
            #endif
            PlayerController.Instance.InvokeRepeating(nameof(PlayerController.Instance.Shoot), 2f, DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(level));
            InvokeRepeating(nameof(InstantiateSpermInInfiniteMode), 2f, DataManagerUtility.GetRepeatRateSpawnForInfiniteMode(level));
        }

        public void LoadDefaultVolume()
        {
            PreferencesManager.Instance.music.SetFloat("music_volume", PlayerPrefs.GetFloat("music_volume", 0f));
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", PlayerPrefs.GetFloat("sfx_volume", 0f));
        }
        private void MuteSounds()
        {
            var storagedVolume = PreferencesManager.Instance.GetMusicVolume();
            var modifiedVolume = DataManagerUtility.GetModifiedVolumeForPause(storagedVolume);
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
                backgroundMusic.volume-=0.1f;
                yield return null;
            }
            backgroundMusic.Stop();
            MuteSounds();
        }
        private IEnumerator UpdateCountDown()
        {
            attempts--;
            
            while (countDown > 0)
            {
                if (cancelCountDown) break;
                ScreensInGameManager.Instance.countDownText.text = $"{countDown--}";
                if(mShowingAd) yield break;
                yield return new WaitForSecondsRealtime(1f);
            }
            if(!mShowingAd) SetGameState(GameState.Ended);
        }
        
        public static GameController Instance { private set; get; }
        public void PauseGame() {
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
        public void PauseGameOnShowAd()
        {
            mShowingAd = true;
            Time.timeScale = 0;
            PreferencesManager.Instance.music.SetFloat("music_volume", -80f);
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", -80f);
            PreferencesManager.Instance.ui.SetFloat("ui_volume", -80f);
        }
        public void ResumeGameOnCloseAd()
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
                case GameState.Playing:
                    Invoke(nameof(LoadDefaultVolume), 0.1f);
                    ScreensInGameManager.Instance.ShowHudElements();
                    Time.timeScale = 1;
                    break;
                case GameState.Ended:
                    if(mScorePosted) return;
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
                case GameState.Retry:
                    MuteSounds();
                    ScreensInGameManager.Instance.ShowRetryScreenElements();
                    ShowRetryScreen();
                    Time.timeScale = 0;
                    break;
            }
        }
    
        private void PlayGameOverAudio()
        {
            if(AudioManager.Instance) AudioManager.Instance.failAudio.Play();
        }
        
        private void PostToLeaderBoard()
        {
            #if UNITY_ANDROID
            PlayGamesController.PostToLeaderBoardEndless(ScoreController.Instance.scoreAmount);
            #endif
            mScorePosted = true;
        }
    
        public void PlayPopAudio()
        {
            if (AudioManager.Instance) AudioManager.Instance.clickAudio.Play();
        }
        
        public void CancelCountDown()
        {
            cancelCountDown = true;
        }
        public bool CheckIfCanRetry()
        {
            return  attempts > 0 && UnityAdsManager.Instance && UnityAdsManager.Instance.canRetry;
        }
        public IEnumerator Retry()
        {
            level = 1;
            limitRate = 0;
            RemoveAllSperms();
            SetGameState(GameState.Playing);
            OvumController.Instance.isInmune = false;
            ResumeGameOnCloseAd();
            yield return null;
        }
        private void ShowRetryScreen()
        {
            StartCoroutine(mStartCountDown);
        }
        public void ShowAdForRetry()
        {
            UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Retry, GameMode.Endless);
        }
        private void RemoveAllSperms()
        {
            var sperms = FindObjectsOfType<SpermController>().Where((sperm) => !sperm.tryToEnter).ToList();
            if (sperms.Count <= 0) return;
            foreach (var sperm in sperms)
            {
                StartCoroutine(DestroySperm(sperm, 0f, "game controller: destroy all sperms"));
            }
        }

        public int rate;
        public int limitRate;
        public GameObject 
            knockOutAnimationWithoutStars, 
            knockOutAnimation;
        public IEnumerator DestroySperm(SpermController objectToDestroy, float time, String location, bool destroyedByPlayer = false, bool spawnOther = false)
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

        public SpermController sperm;
        public void InstantiateSpermInInfiniteMode()
        {
            if(gameState != GameState.Playing) return;
            var xPosition = Random.Range(-DataManagerUtility.GetScreenWidth() * .7f, DataManagerUtility.GetScreenWidth() * .7f);
            var spermInstantiated = Instantiate(sperm, new Vector3(xPosition, DataManagerUtility.GetScreenHeight()), Quaternion.identity);
            spermInstantiated.Life = 1f;
        }
        private static readonly int Intangible = Animator.StringToHash("Intangible");
    
        private void OnGUI()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(gameState == GameState.Playing)
                {
                    PauseGame();
                }
            }
        }
    }
}
