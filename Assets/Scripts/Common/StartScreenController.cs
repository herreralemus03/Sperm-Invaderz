using System;
using TMPro;
using UnityEngine;

namespace Common
{
    public class StartScreenController : MonoBehaviour
    {
        public TextMeshProUGUI appVersion;
        public GameObject creditsDialog, exitDialog;
        public Animator dialogAnimator, exitAppDialogAnimator;
        public static StartScreenController Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            appVersion.text = $"App version v{Application.version}" +
                              $"\nUnity v{Application.unityVersion}";
        }

        private void Start()
        {
            PreferencesManager.Instance.music.SetFloat("music_volume", PlayerPrefs.GetFloat("music_volume", 0f));
            PreferencesManager.Instance.sfx.SetFloat("sfx_volume", PlayerPrefs.GetFloat("sfx_volume", 0f));
        }

        public void OpenCreditsDialog()
        {
            AudioManager.Instance.boingAudio.Play();
            creditsDialog.SetActive(true);
        }

        public void OpenExitAppDialog()
        {
            AudioManager.Instance.boingAudio.Play();
            exitDialog.SetActive(true);
        }
        public void HideCreditsDialog()
        {
            AudioManager.Instance.whistleAudio.Play();
            dialogAnimator.SetTrigger("Exit");
            Invoke(nameof(CloseCreditsDialog), DataManagerUtility.SecondsToFloat(20));
        }

        public void HideExitAppDialog()
        {
            AudioManager.Instance.whistleAudio.Play();
            exitAppDialogAnimator.SetTrigger("Exit");
            Invoke(nameof(CloseExitAppDialog), DataManagerUtility.SecondsToFloat(20));
        }
        public void CloseExitAppDialog()
        {
            exitDialog.SetActive(false);
        }
        public void CloseCreditsDialog()
        {
            creditsDialog.SetActive(false);
        }
        public void ExitApp()
        {
            Application.Quit();
            Invoke(nameof(CloseExitAppDialog), DataManagerUtility.SecondsToFloat(20));
            Debug.Log("App exit");
        }
        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnGUI()
        {
            if (!exitDialog.activeInHierarchy)
            {
                if(Input.GetKeyDown(KeyCode.Escape)) OpenExitAppDialog();
            }
        }
    }
}