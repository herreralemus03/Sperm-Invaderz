/*using System;
using System.Collections;
using System.Linq;
using Arcade;
using Common;
using Invasion;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using PlayerController = Invasion.PlayerController;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class GameController : MonoBehaviour
{
    public GameState gameState;
    
    [Header("Canvas UI")]
    public GameObject scoreScreen; 
    public GameObject retryScreen; 
    public GameObject uiElements; 
    public GameObject pauseScreen; 
    public GameObject clearLevelScreen;
    public GameObject upgradeTabs;
    public Button
        pauseButton,
        duplicateCoins;
    public TextMeshProUGUI
        rewardAmountText,
        levelArcadeText,
        countDownText, 
        levelText;
    public Toggle vibrationToggle;
    public Slider
        musicSliderVolume,
        sfxSliderVolume;
    public AudioSource backgroundMusic;
    [HideInInspector] 
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
        level = SpermSpawnController.Instance.test ? SpermSpawnController.Instance.actualLevel : PlayerPrefs.GetInt("level", 1);
        levelText.text = $"{level}";
        if(gameMode == GameMode.Arcade) levelArcadeText.text = $"{ScoreArcadeMode.Instance.level}";
        mStartCountDown = UpdateCountDown();
        attempts = 1;
        SetGameState(GameState.Playing);
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
        vibrationToggle.isOn = PreferencesManager.Instance.GetVibration();
        musicSliderVolume.value = PreferencesManager.Instance.GetMusicVolume();
        sfxSliderVolume.value = PreferencesManager.Instance.GetSfxVolume();
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
        if (gameMode == GameMode.Arcade) 
            ScoreArcadeMode.Instance.attempts--;
        else
            attempts--;
        
        while (countDown > 0)
        {
            if (cancelCountDown) break;
            countDownText.text = $"{countDown--}";
            if(mShowingAd) yield break;
            yield return new WaitForSecondsRealtime(1f);
        }
        
        //if(gameMode == GameMode.Adventure) AdsManager.Instance.ChangeReward();
        if(!mShowingAd) SetGameState(GameState.Ended);
    }
    private IEnumerator ConvertPlayerIntangible()
    {
        PlayerController.Instance.animator.SetTrigger("Intangible");
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
        if(!scoreScreen || !retryScreen || !uiElements || !pauseScreen) return;
        gameState = state;
        rewardAmountText.text = $"+{DataManagerUtility.FormatAmount(ScoreController.Instance.tempCredits)}\nfree";
        switch (gameState)
        {
            case GameState.Cleared:
                if(mLevelCleared) return;
                if(gameMode == GameMode.Adventure) PowerUp.Instance.animator.SetTrigger("hide");
                duplicateCoins.gameObject.SetActive(UnityAdsManager.Instance.canRewardCoins && ScoreController.Instance.tempCredits > 0);
                MuteSounds();
                pauseButton.gameObject.SetActive(false);
                mLevelCleared = true;
                RemoveAllSperms();
                StartCoroutine(StopMusic());
                //Invoke(nameof(DestroySpermsWithBonus), 0.1f);
                Invoke(nameof(ShowClearGameScreen), 2f);
                break;
            case GameState.Playing:
                LoadDefaultVolume();
                scoreScreen.SetActive(false);
                retryScreen.SetActive(false);
                if(!SpermSpawnController.Instance.infinite) uiElements.SetActive(true);
                pauseScreen.SetActive(false);
                pauseButton.gameObject.SetActive(true);
                Time.timeScale = 1;
                break;
            case GameState.Ended:
                if(mScorePosted) return;
                if(gameMode == GameMode.Adventure) PowerUp.Instance.animator.SetTrigger("hide");
                duplicateCoins.gameObject.SetActive(UnityAdsManager.Instance.canRewardCoins && ScoreController.Instance.tempCredits > 0);
                PostToLeaderBoard();
                pauseButton.gameObject.SetActive(false);
                scoreScreen.SetActive(true);
                retryScreen.SetActive(false);
                uiElements.SetActive(false);
                pauseScreen.SetActive(false);
                upgradeTabs.SetActive(true);
                PlayGameOverAudio();
                StartCoroutine(StopMusic());
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                MuteSounds();
                scoreScreen.SetActive(false);
                retryScreen.SetActive(false);
                uiElements.SetActive(false);
                pauseScreen.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Retry:
                MuteSounds();
                pauseButton.gameObject.SetActive(false);
                scoreScreen.SetActive(false);
                retryScreen.SetActive(true);
                uiElements.SetActive(false);
                pauseScreen.SetActive(false);
                ShowRetryScreen();
                Time.timeScale = 0;
                break;
        }
    }

    private void PlayGameOverAudio()
    {
        if(gameMode == GameMode.Adventure) AudioManager.Instance.levelFail.Play();
        if(gameMode == GameMode.Endless) AudioManager.Instance.failAudio.Play();
        if(gameMode == GameMode.Arcade) AudioManager.Instance.gameOverAudio.Play();
    }

    public GameMode gameMode;
    private void PostToLeaderBoard()
    {
        if (gameMode == GameMode.Endless)
        {
            PlayGamesController.PostToLeaderBoardEndless(ScoreController.Instance.scoreAmount);
            return;
        }

        if (gameMode == GameMode.Arcade)
        {
            PlayGamesController.PostToLeaderBoardArcade(ScoreArcadeMode.Instance.score);
            ScoreArcadeMode.Instance.score = 0L;
            ScoreArcadeMode.Instance.level = 1;
            ScoreArcadeMode.Instance.attempts = 1;
        }
        
        mScorePosted = true;
    }

    public void DuplicateCoins() => UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Coins);
    public void PlayPopAudio()
    {
        if (AudioManager.Instance) AudioManager.Instance.clickAudio.Play();
    }
    public void ShowClearGameScreen()
    {
        if (gameMode == GameMode.Adventure)
        {
            AudioManager.Instance.levelClear.Play();
        }

        if (gameMode == GameMode.Arcade)
        {
            AudioManager.Instance.clearAudio.Play();
        }
        clearLevelScreen.SetActive(true);
        upgradeTabs.SetActive(true);
        scoreScreen.SetActive(false);
        retryScreen.SetActive(false);
        uiElements.SetActive(false);
        pauseScreen.SetActive(false);
        if (gameMode == GameMode.Arcade)
        {
            ScoreArcadeMode.Instance.level++;
            return;
        }
        //if (!rewarded && gameMode == GameMode.Adventure) AdsManager.Instance.ChangeReward();
        //if(AdsManager.Instance) duplicateCoins.gameObject.SetActive(AdsManager.Instance.canLoadAd && ScoreController.Instance.tempCredits > 0);
        if(gameMode == GameMode.Adventure) PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1)+1);
        
    } 
    public void CancelCountDown()
    {
        cancelCountDown = true;
    }
    public bool CheckIfCanRetry()
    {
        return (gameMode == GameMode.Arcade ? ScoreArcadeMode.Instance.attempts : attempts) > 0 && UnityAdsManager.Instance.canRetry;
    }
    public IEnumerator Retry()
    {
        if (gameMode == GameMode.Endless)
        {
            SpermSpawnController.Instance.levelForInfiniteMode = 1;
            SpermSpawnController.Instance.limitRate = 0;
        }
        if (PlayerController.Instance.isHitByBoss) StartCoroutine(ConvertPlayerIntangible());
        if(gameMode == GameMode.Adventure || gameMode == GameMode.Endless) RemoveAllSperms();
        if (gameMode == GameMode.Arcade) RemoveSomeSperms();
        SetGameState(GameState.Playing);
        OvumController.Instance.isInmune = false;
        OvumController.Instance.hasEnterSperm = false;
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
        StartCoroutine(mStartCountDown);
    }
    public void ShowAdForRetry()
    {
        UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Retry);
    }
    public void ShowAdForCoins()
    {
        UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.Coins);
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
    public TextMeshProUGUI textToLoad;
    public GameObject canvasParent;
    public IEnumerator RewardCoins()
    {
        DataManagerUtility.AddCreditsAmount(ScoreController.Instance.tempCredits);
        ScoreController.Instance.textMesh.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
        textToLoad.text = $"+{DataManagerUtility.FormatAmount(Convert.ToInt64(ScoreController.Instance.tempCredits))}";
        ScoreController.Instance.tempCredits = 0;
        Instantiate(textToLoad, canvasParent.transform.position, Quaternion.identity, canvasParent.transform);
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
}*/