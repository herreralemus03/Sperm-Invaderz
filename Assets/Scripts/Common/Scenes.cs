using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Common
{
    public class Scenes : MonoBehaviour
    {
        public static Scenes Instance { private set; get; }
        public Animator wallsAnimation, canvasAnimation, creditsAnimator;
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");

        private void AnimateOnClose()
        {
            //canvasAnimation.SetTrigger("Hide");
            wallsAnimation.SetTrigger(FadeIn);
            //creditsAnimator.SetTrigger("Hide");
        }
        private void Awake()
        {
            Time.timeScale = 1;
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void LoadMain()
        {
            //Time.timeScale = 1f;
            StartCoroutine(LoadMainScene());
        }
    
        public void LoadInfiniteMode()
        {
            AudioManager.Instance.PlaySplashAudio();
            StartCoroutine(LoadInfinite());
        }
        public void LoadArcadeMode()
        {
            AudioManager.Instance.PlaySplashAudio();
            StartCoroutine(LoadArcade());
        }

        private IEnumerator LoadMainScene()
        {
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene("StartScene");
            if(Arcade.ScoreArcadeMode.Instance) Destroy(Arcade.ScoreArcadeMode.Instance.gameObject);
            yield return null;
        }

        private IEnumerator LoadInfinite()
        {
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene("InfiniteScene");
            yield return null;
        }
    
        private IEnumerator LoadArcade()
        {
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene("ClassicScene");
            yield return null;
        }
    
        private IEnumerator ReloadSceneWithTransitionAnimation()
        {
            AnimateOnClose();
            yield return new WaitForSecondsRealtime(1f);
            if (!ShowInterstitialAd(SceneManager.GetActiveScene().name)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    
        private IEnumerator PlayNextSceneWithTransitionAnimation()
        {
            AnimateOnClose();
            yield return new WaitForSecondsRealtime(1f);
            var level = PlayerPrefs.GetInt("level", 1);
            if (level % 10 == 0)
            {
                LoadBossLevelWithAd();
            }
            else
            {
                LoadNormalLevelWithAd();
            }

            yield return null;
        }
        public void LoadNextLevel()
        {
            //AdsManager.Instance.SubscribeRewardAdForRetry();
            StartCoroutine(PlayNextSceneWithTransitionAnimation());/*
        var level = PlayerPrefs.GetInt("level", 1);
        if (level % 10 == 0)
        {
            LoadBossLevelWithAd();
        }
        else
        {
            LoadNormalLevelWithAd();
        }*/
        }

        public void LoadLevelWithoutAd()
        {
            StartCoroutine(LoadCampaignLevel());
        }

        private IEnumerator LoadCampaignLevel()
        {
            AudioManager.Instance.PlaySplashAudio();
            //AdsManager.Instance.SubscribeRewardAdForRetry();
        
            wallsAnimation.SetTrigger(FadeIn);
        
            yield return new WaitForSeconds(1f);
        
            var level = PlayerPrefs.GetInt("level", 1);
            if (level % 10 == 0)
            {
                LoadBossLevelWithoutAd();
            }
        
            else
            {
                LoadNormalLevelWithoutAd();
            }
        }
        public void ReloadGameScene()
        {
            //AdsManager.Instance.SubscribeRewardAdForRetry();
            StartCoroutine(ReloadSceneWithTransitionAnimation());/*
        if (ShowInterstitialAd(SceneManager.GetActiveScene().name)) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);*/
        }
        private static void LoadNormalLevelWithAd()
        {
            if (ShowInterstitialAd("GameScene")) return;
            SceneManager.LoadScene("GameScene");
        }
        private static void LoadBossLevelWithAd()
        {
            if (ShowInterstitialAd("BossScene")) return;
            SceneManager.LoadScene("BossScene");
        }

        private static void LoadNormalLevelWithoutAd() => SceneManager.LoadScene("GameScene");
        private static void LoadBossLevelWithoutAd() => SceneManager.LoadScene("BossScene");
        private static bool ShowInterstitialAd(string scene)
        {
            var load = Random.Range(1, 3) == 1;
            if (!UnityAdsManager.Instance || !UnityAdsManager.Instance.canShowInterstitial || !load) return false;
            UnityAdsManager.Instance.ShowInterstitialAd(scene);
            return true;
        }
    }
}