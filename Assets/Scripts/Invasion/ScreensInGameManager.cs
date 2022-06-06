using System;
using System.Collections;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Invasion
{
    public class ScreensInGameManager : MonoBehaviour
    {
        [Header("Animator")] public Animator screenGroupAnimator;
        [Header("HUD")] 
        public Button pauseButton;
        public Slider fertilitySlider;
        public Slider powerUpSlider;
        public ScoreController scoreController;
        public GameObject levelParent;
        public TextMeshProUGUI levelLabel;
        [Header("Clear level")] 
        public Button nextLevelButton;
        [Header("Fail level")] 
        public Button reloadLevelButton;
        [Header("Retry screen")]
        public Button retryButton;
        public Button cancelButton;
        public TextMeshProUGUI description;
        public TextMeshProUGUI countDownText;
        public GameObject heroAnimator;
        [Header("Pause Screen")] 
        public Button resumeGameButton;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle vibrationToggle;
        [Header("Common")] 
        public GameObject creditsParent;
        public Animator creditsAnimator;
        public TextMeshProUGUI credits;
        public TextMeshProUGUI title;
        public Button homeButton;
        public GameObject dialogBox;
        [Header("Common level finish")]
        public UpgradesController upgradesDialog;
        public GameObject rewardCoinsParent;
        public TextMeshProUGUI rewardAmount;
        public GameObject panel;
        public TextMeshProUGUI titlePanel;
        public GameObject titlePanelParent;
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
            fertilitySlider.gameObject.SetActive(show);
            powerUpSlider.gameObject.SetActive(show);
            scoreController.gameObject.SetActive(show);
            levelParent.gameObject.SetActive(show);
        }
        private void PauseScreenElements(bool show)
        {
            creditsParent.SetActive(show);
            if (show) credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            resumeGameButton.gameObject.SetActive(show);
            homeButton.gameObject.SetActive(show);
            panel.SetActive(show);
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            if(show) titlePanel.text = "SETTINGS";
            title.gameObject.SetActive(show);
            if(show) title.text = "PAUSED";
            musicSlider.gameObject.SetActive(show);
            sfxSlider.gameObject.SetActive(show);
            vibrationToggle.gameObject.SetActive(show);
            dialogBox.SetActive(show);
        }
        private void RetryScreenElements(bool show)
        {             
            creditsParent.SetActive(show);
            if (show) credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            dialogBox.SetActive(show);
            retryButton.gameObject.SetActive(show);
            cancelButton.gameObject.SetActive(show);
            description.gameObject.SetActive(show);
            countDownText.gameObject.SetActive(show);
            heroAnimator.gameObject.SetActive(show);
            panel.SetActive(show);
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            if(show) titlePanel.text = "SUPER BPC";
            title.gameObject.SetActive(show);
            if(show) title.text = "RETRY";
        }
        private void ClearScreenElements(bool show, long rewards = 0L)
        { 
            creditsParent.SetActive(show);
            if (show) credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            dialogBox.SetActive(show);
            nextLevelButton.gameObject.SetActive(show);
            homeButton.gameObject.SetActive(show);
            if (rewards > 0L)
            {
                rewardCoinsParent.gameObject.SetActive(show);
                rewardAmount.text = $"{DataManagerUtility.FormatAmount(rewards)}";
            }
            upgradesDialog.gameObject.SetActive(show);
            panel.SetActive(show);
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            if(show) titlePanel.text = "UPGRADES";
            title.gameObject.SetActive(show);
            if(show) title.text = "LEVEL CLEAR";
        }
        private void GameOverScreenElements(bool show, long rewards = 0L)
        {        
            creditsParent.SetActive(show);  
            if (show) credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();   
            dialogBox.SetActive(show);
            reloadLevelButton.gameObject.SetActive(show);
            homeButton.gameObject.SetActive(show);
            rewardCoinsParent.gameObject.SetActive(show);
            if (rewards > 0L)
            {
                rewardCoinsParent.gameObject.SetActive(show);
                rewardAmount.text = $"{DataManagerUtility.FormatAmount(rewards)}";
            }
            upgradesDialog.gameObject.SetActive(show);
            panel.SetActive(show);
            titlePanelParent.SetActive(show);
            titlePanel.gameObject.SetActive(show);
            if(show) titlePanel.text = "UPGRADES";
            title.gameObject.SetActive(show);
            if(show) title.text = "LEVEL FAIL";
        }

        public void ShowPauseScreen()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> RetryScreenElements(false),
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
            Action hideScreen4,
            Action showScreen)
        {
            screenGroupAnimator.SetTrigger(Hide);
            yield return new WaitForSecondsRealtime(0.5f);
            hideScreen1();
            hideScreen2();
            hideScreen3();
            hideScreen4();
            showScreen();
        }
        
        public void ShowRetryScreenElements()
        {
            StartCoroutine(MakeScreenTransition(
            ()=> PauseScreenElements(false),
            ()=> ClearScreenElements(false),
            ()=> GameOverScreenElements(false),
            ()=> HudElements(false),
            ()=> RetryScreenElements(true)
                ));
        }
        public void ShowClearScreenElements(long reward)
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> RetryScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(false),
                ()=> ClearScreenElements(true, reward)
            ));
        }
        public void ShowGameOverScreenElements(long reward)
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> RetryScreenElements(false),
                ()=> ClearScreenElements(false),
                ()=> HudElements(false),
                ()=> GameOverScreenElements(true, reward)
            ));
        }
        public void ShowHudElements()
        {
            StartCoroutine(MakeScreenTransition(
                ()=> PauseScreenElements(false),
                ()=> RetryScreenElements(false),
                ()=> ClearScreenElements(false),
                ()=> GameOverScreenElements(false),
                ()=> HudElements(true)
            ));
        }
    }
}