using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

namespace Common
{
    public class UnityAdsManager : MonoBehaviour, IUnityAdsListener
    {
        public GameMode gameMode;
        public static UnityAdsManager Instance { private set; get; }
        private string mSceneToLoad;
        public string gameId = "3889341";
        public string interstitialId = "video";
        private const string 
            RewardRetry = "rewardRetry", 
            RewardCoins = "rewardCoins", 
            RewardPowerUpgrade = "rewardPower", 
            RewardSpeedUpgrade = "rewardSpeed", 
            RewardEarnsUpgrade = "rewardEarns";
        public bool 
            canRetry, 
            canRewardCoins,
            canRewardPowerUpgrade,
            canRewardSpeedUpgrade,
            canRewardEarnsUpgrade,
            canShowInterstitial;
        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
        private void Start ()
        {
            /*var level = 20;
            PlayerPrefs.SetInt("power", level);
            PlayerPrefs.SetInt("speed", level);
            PlayerPrefs.SetInt("earns", level);
            PlayerPrefs.SetInt("level", level);
            PlayerPrefs.SetString("credits", $"{10000L}");
            PlayerPrefs.SetString("power_price", $"{10L}");
            PlayerPrefs.SetString("speed_price", $"{10L}");
            PlayerPrefs.SetString("earns_price", $"{10L}");*/
            // UnityEditor.Advertisements.AdvertisementSettings.enabled = true;
            // UnityEditor.Advertisements.AdvertisementSettings.initializeOnStartup = false;
            Advertisement.AddListener (this);
            InitializeAd();
        }
        private void InitializeAd()
        {
            canRetry = false;
            canRewardCoins = false;
            Advertisement.Initialize (gameId, Application.isEditor || Debug.isDebugBuild);
        }
        public void ShowRewardedVideo(AdRewardType adRewardType, GameMode gameMode)
        {
            this.gameMode = gameMode;
            switch (adRewardType)
            {
                case AdRewardType.Retry:
                    ShowRewardedAd(RewardRetry);
                    break;
                case AdRewardType.Coins:
                    ShowRewardedAd(RewardCoins);
                    break;
                case AdRewardType.PowerUpgrade:
                    ShowRewardedAd(RewardPowerUpgrade);
                    break;
                case AdRewardType.SpeedUpgrade:
                    ShowRewardedAd(RewardSpeedUpgrade);
                    break;
                case AdRewardType.EarnsUpgrade:
                    ShowRewardedAd(RewardEarnsUpgrade);
                    break;
            }
        }
        public void ShowInterstitialAd(string sceneToLoad)
        {
            mSceneToLoad = sceneToLoad;
            Advertisement.Show(interstitialId);
        }
        private void ShowRewardedAd(string id)
        {
            if (!Advertisement.IsReady(id)) return;
            Advertisement.Show(id);
        }
    
        public void OnUnityAdsDidFinish (string placementId, ShowResult showResult)
        {
            if (placementId == interstitialId)
            {
                SceneManager.LoadScene(mSceneToLoad);
                return;
            }
        
            switch (showResult)
            {
                case ShowResult.Finished:
                    ApplyReward(placementId);
                    break;
                case ShowResult.Skipped when (gameMode == GameMode.Adventure && placementId == RewardRetry):
                    Invasion.GameController.Instance.SetGameState(GameState.Ended);
                    break;
                case ShowResult.Skipped when (gameMode == GameMode.Endless && placementId == RewardRetry):
                    Endless.GameController.Instance.SetGameState(GameState.Ended);
                    break;
                case ShowResult.Skipped when (gameMode == GameMode.Arcade && placementId == RewardRetry):
                    //Arcade.GameController.Instance.SetGameState(GameState.Ended);
                    break;
                case ShowResult.Failed when (gameMode == GameMode.Adventure && placementId == RewardRetry):
                    Invasion.GameController.Instance.SetGameState(GameState.Ended);
                    break;
                case ShowResult.Failed when (gameMode == GameMode.Endless && placementId == RewardRetry):
                    Endless.GameController.Instance.SetGameState(GameState.Ended);
                    break;
                case ShowResult.Failed when (gameMode == GameMode.Arcade && placementId == RewardRetry):
                    Arcade.GameController.Instance.SetGameState(GameState.Ended);
                    break;
            }
            InitializeAd();
        }

        private void ApplyReward(string id)
        {
            switch (id)
            {
                case RewardRetry when gameMode == GameMode.Adventure:
                    Invasion.GameController.Instance.StartCoroutine(Invasion.GameController.Instance.Retry());
                    break;
                case RewardCoins when gameMode == GameMode.Adventure:
                    Invasion.GameController.Instance.StartCoroutine(Invasion.GameController.Instance.RewardCoins());
                    break;
                case RewardPowerUpgrade when gameMode == GameMode.Adventure:
                    //Invasion.UpgradesController.Instance.UpgradeShootPowerFree();
                    Invasion.UpgradesController.Instance.UpgradedCallBack(UpgradeType.Power);
                    break;
                case RewardSpeedUpgrade when gameMode == GameMode.Adventure:
                    //Invasion.UpgradesController.Instance.UpgradeShootSpeedRateFree();
                    Invasion.UpgradesController.Instance.UpgradedCallBack(UpgradeType.Speed);
                    break;
                case RewardEarnsUpgrade when gameMode == GameMode.Adventure:
                    //Invasion.UpgradesController.Instance.UpgradeEarnsFree();
                    Invasion.UpgradesController.Instance.UpgradedCallBack(UpgradeType.Earns);
                    break;
                case RewardRetry when gameMode == GameMode.Endless:
                    Endless.GameController.Instance.StartCoroutine(Endless.GameController.Instance.Retry());
                    break;
                case RewardRetry when gameMode == GameMode.Arcade:
                    Arcade.GameController.Instance.StartCoroutine(Arcade.GameController.Instance.IncrementLife());
                    break;
                    
            }
        }

        public void OnUnityAdsReady (string placementId) {
            if (placementId == RewardRetry) {
                canRetry = true;
            }
            if (placementId == RewardCoins)
            {
                canRewardCoins = true;
            }
            if (placementId == interstitialId)
            {
                canShowInterstitial = true;
            }

            if (placementId == RewardPowerUpgrade)
            {
                canRewardPowerUpgrade = true;
            }
        
            if (placementId == RewardSpeedUpgrade)
            {
                canRewardSpeedUpgrade = true;
            }
            if (placementId == RewardEarnsUpgrade)
            {
                canRewardEarnsUpgrade = true;
            }
        }

        public void OnUnityAdsDidError (string message) {
            if (!Advertisement.IsReady(RewardRetry) || !Advertisement.IsReady(RewardCoins))
            {
                InitializeAd();
            }
        }

        public void OnUnityAdsDidStart (string placementId) {
            if(placementId == RewardRetry && gameMode == GameMode.Adventure) Invasion.GameController.Instance.PauseGameOnShowAd();
            if(placementId == RewardRetry && gameMode == GameMode.Endless) Endless.GameController.Instance.PauseGameOnShowAd();
            //if(placementId == RewardRetry && gameMode == GameMode.Arcade) Arcade.GameController.Instance.PauseGameOnShowAd();
        } 

        public void OnDestroy() {
            Advertisement.RemoveListener(this);
        }
    }
}