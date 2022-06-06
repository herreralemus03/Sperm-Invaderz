using UnityEngine;
using Common;
namespace Arcade
{
    public class LifeReward : MonoBehaviour
    {
        public Animator animator;
        
        public void HideDialog()
        {
            if(AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
            animator.SetTrigger(Exit);
            Invoke(nameof(CloseExitAppDialog), DataManagerUtility.SecondsToFloat(20));
        }

        public void RefreshCredits()
        {
            if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
            LifeController.Instance.amount.text = $"{ScoreArcadeMode.Instance.playerLife}";
            ScreensInGameManager.Instance.lifeLabel2.text = $"{ScoreArcadeMode.Instance.playerLife}";
            ScreensInGameManager.Instance.lifeLabelAnimator2.SetTrigger(Increase);
            if(AudioManager.Instance) AudioManager.Instance.lifeUp.Play();
        }
        
        private static readonly int Increase = Animator.StringToHash("Increase");
        private static readonly int Exit = Animator.StringToHash("Exit");

        public void CloseExitAppDialog()
        {
            RefreshCredits();
            Destroy(gameObject);
        }
    }
}