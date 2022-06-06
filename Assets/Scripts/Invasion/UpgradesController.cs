using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Invasion
{
    public class UpgradesController : MonoBehaviour
    {
        public static UpgradesController Instance { private set; get; }
        public Button shootPowerButton, shootSpeedRateButton, earnsButton;

        public TextMeshProUGUI shootPowerPriceLabel,
            shootSpeedPriceLabel,
            earnsPriceLabel,
            shootPowerLevel,
            shootSpeedLevel,
            earnsLevel;

        public Animator powerLevelLabel, speedLevelLabel, earnsLevelLabel;
        public bool canRewardPowerFree;
        public bool canRewardSpeedFree;
        public bool canRewardEarnsFree;

        public GameObject
            powerUpgradeChild,
            speedUpgradeChild,
            earnsUpgradeChild,
            powerUpgradeFreeChild,
            speedUpgradeFreeChild,
            earnsUpgradeFreeChild;

        public List<UpgradeButton> upgradesButton;

        private void Awake()
        {
            if (Instance == null) Instance = this;

            UpdatePriceValue("power", shootPowerPriceLabel, shootPowerLevel);
            UpdatePriceValue("speed", shootSpeedPriceLabel, shootSpeedLevel);
            UpdatePriceValue("earns", earnsPriceLabel, earnsLevel);
        }

        private void Start()
        {
            ScreensInGameManager.Instance.credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            CheckIfCanUpgradeComponent();
        }

        public void CheckIfCanUpgradeComponent()
        {
            var powerPrice = GetComponentPrice("power");
            var speedPrice = GetComponentPrice("speed");
            var earnsPrice = GetComponentPrice("earns");

            var credits = DataManagerUtility.GetCreditsAmount();
            if(!canRewardPowerFree) shootPowerButton.interactable = powerPrice <= credits;
            if(!canRewardSpeedFree) shootSpeedRateButton.interactable = speedPrice <= credits;
            if(!canRewardEarnsFree) earnsButton.interactable = earnsPrice <= credits;

            if (!canRewardComponentFree) return;

            var upgradeButtonToRewardFree = GetUpgradeButtonToReward();
            if (upgradeButtonToRewardFree == null || upgradeButtonToRewardFree.upgradeType == UpgradeType.None) return;
            switch (upgradeButtonToRewardFree.upgradeType)
            {
                case UpgradeType.Power:
                    if(!UnityAdsManager.Instance || !UnityAdsManager.Instance.canRewardPowerUpgrade) break;
                    canRewardPowerFree = true;
                    shootPowerButton.interactable = true;
                    ChangeUpgradeButtonChild(powerUpgradeChild, powerUpgradeFreeChild);
                    return;
                case UpgradeType.Speed:
                    if(!UnityAdsManager.Instance || !UnityAdsManager.Instance.canRewardSpeedUpgrade) break;
                    canRewardSpeedFree = true;
                    shootSpeedRateButton.interactable = true;
                    ChangeUpgradeButtonChild(speedUpgradeChild, speedUpgradeFreeChild);
                    return;
                case UpgradeType.Earns:
                    if(!UnityAdsManager.Instance || !UnityAdsManager.Instance.canRewardEarnsUpgrade) break;
                    canRewardEarnsFree = true;
                    earnsButton.interactable = true;
                    ChangeUpgradeButtonChild(earnsUpgradeChild, earnsUpgradeFreeChild);
                    return;
            }
        }

        private void ChangeUpgradeButtonChild(GameObject upgradeButtonChild, GameObject upgradeButtonFreeChild)
        {
            upgradeButtonChild.SetActive(false);
            upgradeButtonFreeChild.SetActive(true);
            canRewardComponentFree = false;
        }

        public bool CheckIfCanRewardComponentFree()
        {
            var buttonsNotInteractable = upgradesButton.Where((button) => !button.button.interactable).ToList();
            return buttonsNotInteractable.Any();
        }

        public UpgradeButton GetUpgradeButtonToReward()
        {
            if (!CheckIfCanRewardComponentFree()) return null;
            var factor = Random.Range(1, upgradesButton.Count);
            return upgradesButton[factor];
        }

        public bool rewardFreeShowed;
        public AdRewardType adRewardType = AdRewardType.EarnsUpgrade;
        public bool canRewardComponentFree = true;

        public void PlaySwitchAudio()
        {
            if (AudioManager.Instance) AudioManager.Instance.switchTabAudio.Play();
        }

        private void LockFreeRewards()
        {
            canRewardPowerFree = false;
            canRewardSpeedFree = false;
            canRewardEarnsFree = false;
            ChangeUpgradeButtonChild(powerUpgradeFreeChild, powerUpgradeChild);
            ChangeUpgradeButtonChild(speedUpgradeFreeChild, speedUpgradeChild);
            ChangeUpgradeButtonChild(earnsUpgradeFreeChild, earnsUpgradeChild);
        }

        public void UpgradeShootPower()
        {
            shootPowerButton.interactable = false;
            if (canRewardPowerFree)
            {
                UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.PowerUpgrade, GameMode.Adventure);
                return;
            }
            Upgrade("power", shootPowerPriceLabel, shootPowerLevel, false);
        }

        public void UpgradeShootSpeedRate()
        {
            shootSpeedRateButton.interactable = false;
            if (canRewardSpeedFree)
            {
                UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.SpeedUpgrade, GameMode.Adventure);
                return;
            }
            Upgrade("speed", shootSpeedPriceLabel, shootSpeedLevel, false);
        }

        public void UpgradeEarns()
        {
            earnsButton.interactable = false;
            if (canRewardEarnsFree)
            {
                UnityAdsManager.Instance.ShowRewardedVideo(AdRewardType.EarnsUpgrade, GameMode.Adventure);
                return;
            }

            Upgrade("earns", earnsPriceLabel, earnsLevel, false);
        }

        public void UpgradedCallBack(UpgradeType upgradeType)
        {
           if(AudioManager.Instance) AudioManager.Instance.boingAudio2.Play();
           var dialog = Instantiate(upgradeRewardDialog, GameController.Instance.rewardParent.transform.position, Quaternion.identity, GameController.Instance.rewardParent.transform);
           dialog.UpgradeType = upgradeType;
        }

        public UpgradeReward upgradeRewardDialog;
        public void UpgradeShootPowerFree()
        {
            Upgrade("power", shootPowerPriceLabel, shootPowerLevel, true);
        }
        
        public void UpgradeShootSpeedRateFree()
        {
            Upgrade("speed", shootSpeedPriceLabel, shootSpeedLevel, true);
        }
        public void UpgradeEarnsFree()
        {
            Upgrade("earns", earnsPriceLabel, earnsLevel, true);
        }

        private void Upgrade(string component, TextMeshProUGUI priceLabel, TextMeshProUGUI levelLabel, bool freeUpgrade)
        {
            if(AudioManager.Instance) AudioManager.Instance.upgradeAudio.Play();
            var componentPrice = GetComponentPrice(component);
            if(!freeUpgrade) DataManagerUtility.SubtractCreditsAmount(componentPrice);
            ScoreController.Instance.scoreLabel.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            ScreensInGameManager.Instance.credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            SetComponentPrice(component);

            UpdatePriceValue(component, priceLabel, levelLabel);
            if (freeUpgrade)
            {
                LockFreeRewards();
                canRewardComponentFree = false;
            }

            switch (component)
            {
                case "power":
                    powerLevelLabel.SetTrigger(Increase);
                    break;
                case "speed":
                    speedLevelLabel.SetTrigger(Increase);
                    break;
                case "earns":
                    earnsLevelLabel.SetTrigger(Increase);
                    break;
            }
            
            CheckIfCanUpgradeComponent();
        }
        
        private static readonly int Increase = Animator.StringToHash("Increase");
        private void UpdatePriceValue(string component, TextMeshProUGUI priceLabel, TextMeshProUGUI levelLabel)
        {
            priceLabel.text = $"{DataManagerUtility.FormatAmount(GetComponentPrice(component))}";
            levelLabel.text = $"Lvl. {PlayerPrefs.GetInt($"{component}", 1)}";
        }
    
        private void OnDestroy()
        {
            Instance = null;
        }    
        private void OnEnable()
        {
            CheckIfCanUpgradeComponent();
            PlayerController.Instance.gameObject.SetActive(false);
        }

        public long GetComponentPrice(string componentName)
        {
            var amount = Convert.ToInt64(PlayerPrefs.GetString($"{componentName}_price", "100"));
            return amount;
        }
    
        public void SetComponentPrice(string componentName)
        {
            var componentLevel = PlayerPrefs.GetInt(componentName, 1);
            PlayerPrefs.SetInt(componentName, (componentLevel + 1));
            PlayerPrefs.SetString($"{componentName}_price", Mathf.RoundToInt(GetComponentPrice(componentName) * 1.15f).ToString());
        }
    }
    [Serializable]
    public class UpgradeButton
    {
        public UpgradeType upgradeType;
        public Button button;
    }
}