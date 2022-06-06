using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Invasion
{
    public class UpgradeReward : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI prevLevelText;
        public TextMeshProUGUI nextLevelText;
        public TextMeshProUGUI titleUpgradeReward;
        public UpgradeType UpgradeType
        {
            get => mUpgradeType;
            set
            {
                mUpgradeType = value;
                switch (value)
                {
                    case UpgradeType.Power:
                        prevLevelText.text = $"Lvl. {PlayerPrefs.GetInt("power", 1)}";
                        nextLevelText.text = $"Lvl. {PlayerPrefs.GetInt("power", 1) + 1 }";
                        titleUpgradeReward.text = "SHOOT FORCE UPGRADED";
                        icon.sprite = powerIcon;
                        break;
                    case UpgradeType.Speed:
                        prevLevelText.text = $"Lvl. {PlayerPrefs.GetInt("speed", 1)}";
                        nextLevelText.text = $"Lvl. {PlayerPrefs.GetInt("speed", 1) + 1 }";
                        titleUpgradeReward.text = "SHOOT SPEED UPGRADED";
                        icon.sprite = speedIcon;
                        break;
                    case UpgradeType.Earns:
                        prevLevelText.text = $"Lvl. {PlayerPrefs.GetInt("earns", 1)}";
                        nextLevelText.text = $"Lvl. {PlayerPrefs.GetInt("earns", 1) + 1 }";
                        titleUpgradeReward.text = "EARNS PLUGIN UPGRADED";
                        icon.sprite = earnsIcon;
                        break;
                }
            }
        }

        private UpgradeType mUpgradeType;
        public Image icon;
        public Sprite powerIcon, speedIcon, earnsIcon;
        private static readonly int Exit = Animator.StringToHash("Exit");

        public void HideDialog()
        {
            if(AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
            animator.SetTrigger(Exit);
            RefreshUpgrade();
            Invoke(nameof(CloseExitAppDialog), DataManagerUtility.SecondsToFloat(20));
        }

        public void RefreshUpgrade()
        {
            switch (UpgradeType)
            {
                case UpgradeType.Power:
                    UpgradesController.Instance.UpgradeShootPowerFree();
                    break;
                case UpgradeType.Speed:
                    UpgradesController.Instance.UpgradeShootSpeedRateFree();
                    break;
                case UpgradeType.Earns:
                    UpgradesController.Instance.UpgradeEarnsFree();
                    break;
            }
            
        }
        
        public void CloseExitAppDialog()
        {
            Destroy(gameObject);
        }
    }
}