using Common;
using TMPro;
using UnityEngine;

namespace Invasion
{
    public class RewardCreditsText : MonoBehaviour
    {
        public TextMeshProUGUI rewardAmount;
        public Animator animator;
        private static readonly int Exit = Animator.StringToHash("Exit");

        public void HideDialog()
        {
            if(AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
            animator.SetTrigger(Exit);
            Invoke(nameof(CloseExitAppDialog), DataManagerUtility.SecondsToFloat(20));
        }

        public void RefreshCredits()
        {
            if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
            ScoreController.Instance.scoreLabel.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            ScreensInGameManager.Instance.credits.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
            ScreensInGameManager.Instance.creditsAnimator.SetTrigger(Increase);
        }
        
        private static readonly int Increase = Animator.StringToHash("Increase");
        public void CloseExitAppDialog()
        {
            RefreshCredits();
            Destroy(gameObject);
        }
    }
}
