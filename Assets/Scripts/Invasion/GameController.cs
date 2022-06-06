using System;
using System.Collections;
using System.Linq;
using Common;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

namespace Invasion
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
            upgradeTabs;*/
        /*public Button
            pauseButton,
            duplicateCoins;*/
        /*public TextMeshProUGUI
            rewardAmountText,
            countDownText, 
            levelText;*/
        /*public Toggle vibrationToggle;*/
        /*public Slider
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
            level = PlayerPrefs.GetInt("level", 1);
            mStartCountDown = UpdateCountDown();
            attempts = 1;
        }
        private void Start()
        {
            ScreensInGameManager.Instance.levelLabel.text = $"{level}";
            LoadDefaultVolume();
            PlayerController.Instance.ChangePlayerColor(PlayerController.Instance.normalColor);
            var repeatRate = DataManagerUtility.GetShotSpeed(PlayerPrefs.GetInt("speed", 1));
            PlayerController.Instance.InvokeRepeating(nameof(PlayerController.Instance.Shoot), 2f, repeatRate);
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
            
            //if(gameMode == GameMode.Adventure) AdsManager.Instance.ChangeReward();
            if(!mShowingAd) SetGameState(GameState.Ended);
        }
        private IEnumerator ConvertPlayerIntangible()
        {
            PlayerController.Instance.playerAnimator.SetTrigger(Intangible);
            yield return new WaitForSeconds(2f);
            PlayerController.Instance.isHitByBoss = false;
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
            if(ScoreController.Instance) ScreensInGameManager.Instance.rewardAmount.text = $"+{DataManagerUtility.FormatAmount(ScoreController.Instance.tempCredits)}\nfree";
            switch (gameState)
            {
                case GameState.Cleared:
                    if(mLevelCleared) return;
                    MuteSounds();
                    StartCoroutine(StopMusic());
                    mLevelCleared = true;
                    RemoveAllSperms();
                    Invoke(nameof(ShowClearGameScreen), 2f);
                    break;
                case GameState.Playing:
                    LoadDefaultVolume();
                    ScreensInGameManager.Instance.ShowHudElements();
                    Time.timeScale = 1;
                    break;
                case GameState.Ended:
                    if(mScorePosted) return;
                    ScreensInGameManager.Instance.ShowGameOverScreenElements(ScoreController.Instance.tempCredits);
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
                    ShowRetryScreen();
                    Time.timeScale = 0;
                    break;
            }
        }
        private void PlayGameOverAudio()
        {
           if(AudioManager.Instance) AudioManager.Instance.levelFail.Play();
        }

        public void DuplicateCoins() => UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Coins, GameMode.Adventure);
        public void PlayPopAudio()
        {
            if (AudioManager.Instance) AudioManager.Instance.clickAudio.Play();
        }
        public void ShowClearGameScreen()
        {
            if(AudioManager.Instance) AudioManager.Instance.levelClear.Play();
            ScreensInGameManager.Instance.ShowClearScreenElements(ScoreController.Instance.tempCredits);
            
            //duplicateCoins.gameObject.SetActive(UnityAdsManager.Instance && UnityAdsManager.Instance.canRewardCoins && ScoreController.Instance.tempCredits > 0);
            /*clearLevelScreen.SetActive(true);
            upgradeTabs.SetActive(true);
            scoreScreen.SetActive(false);
            retryScreen.SetActive(false);
            uiElements.SetActive(false);
            pauseScreen.SetActive(false);*/
            
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1)+1);
        } 
        public void CancelCountDown()
        {
            cancelCountDown = true;
        }
        public bool CheckIfCanRetry()
        {
            return attempts > 0 && UnityAdsManager.Instance && UnityAdsManager.Instance.canRetry;
        }
        public IEnumerator Retry()
        {            
            if (PlayerController.Instance.isHitByBoss) StartCoroutine(ConvertPlayerIntangible());
            RemoveAllSperms();
            SetGameState(GameState.Playing);
            OvumController.Instance.isInmune = false;
            OvumController.Instance.alreadyFertilized = false;
            ResumeGameOnCloseAd();
            yield return null;
        }
        private void RemoveSomeSperms()
        {
            foreach (var sperm in FindObjectsOfType<SpermController>().Where((sperm)=> sperm.isInOvum))
            {
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(sperm, 0f, "game controller: destroy some sperms 1"));
            }
    
            foreach (var sperm in FindObjectsOfType<SpermController>().Where((sperm)=> sperm.separated))
            {
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(sperm, 0f, "game controller: destroy some sperms 2"));
            }
        }
        private void ShowRetryScreen()
        {
            ScreensInGameManager.Instance.ShowRetryScreenElements();
            StartCoroutine(mStartCountDown);
        }
        public void ShowAdForRetry()
        {
            UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Retry, GameMode.Adventure);
        }
        public void ShowAdForCoins()
        {
            if(UnityAdsManager.Instance) UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Coins, GameMode.Adventure);
            else ScreensInGameManager.Instance.rewardCoinsParent.SetActive(false);
        }
        private void RemoveAllSperms()
        {
            var sperms = FindObjectsOfType<SpermController>().Where((sperm) => !sperm.tryToEnter).ToList();
            if (sperms.Count <= 0) return;
            foreach (var sperm in sperms)
            {
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(sperm, 0f, "game controller: destroy all sperms"));
            }
        }
        private void DestroySpermsWithBonus()
        {
            var earn = DataManagerUtility.GetShotEarns(PlayerPrefs.GetInt("earns", 1));
            var sperms = FindObjectsOfType<SpermController>().Where(spermX => spermX.isDestroyed == false).ToList();
            foreach (var sperm in sperms)
            {
                if(sperm == null) return;
                ScoreController.Instance.InstantiateScoreText(sperm.transform.position, Mathf.RoundToInt(earn));
                SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(sperm, 0f, "game controller: destroy all sperms with bonus"));
            }
        }
        private void OnDestroy()
        {
            Instance = null;
        }
        public RewardCreditsText textToLoad;
        public GameObject rewardParent;
        public GameObject canvasParent;
        private static readonly int Intangible = Animator.StringToHash("Intangible");

        public IEnumerator RewardCoins()
        {
            DataManagerUtility.AddCreditsAmount(ScoreController.Instance.tempCredits);
            ScoreController.Instance.scoreLabel.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            textToLoad.rewardAmount.text = $"+{DataManagerUtility.FormatAmount(Convert.ToInt64(ScoreController.Instance.tempCredits))}";
            ScoreController.Instance.tempCredits = 0;
            Instantiate(textToLoad, rewardParent.transform.position, Quaternion.identity, rewardParent.transform);
            if(AudioManager.Instance) AudioManager.Instance.boingAudio2.Play();
            if(UpgradesController.Instance) UpgradesController.Instance.Invoke("CheckIfCanUpgradeComponent", 0.1f);
            yield return null;
        }
    
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
