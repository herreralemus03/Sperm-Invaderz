/*﻿using System;
 using System.Collections;
 using UnityEngine;
using GoogleMobileAds.Api;
 using UnityEngine.SceneManagement;
 
 public enum RewardType { Coins, Attempt }
public class AdsManager : MonoBehaviour
{
    public RewardType rewardType;
    public bool canLoadAd, interstitialAdCanLoad, rewardEventsSubscribed, interstitialAdEventsSubscribed, isDebug, retryRewarded;
    public static AdsManager Instance { private set; get; }
    
    private RewardBasedVideoAd mRewardBasedVideoAd;
    private InterstitialAd mInterstitialAd;
    private string mSceneToLoad;
    
    private const string 
        TestAdUnitId = "ca-app-pub-3940256099942544/5224354917", 
        TestAdUnitInterstitialId = "ca-app-pub-3940256099942544/1033173712",
        //RewardedAdId = "ca-app-pub-2752717855926930/9161260912",
        RewardedAdId = "ca-app-pub-4384579203907671/8463463746",
        //InterstitialId = "ca-app-pub-2752717855926930/8750197379";
        InterstitialId = "ca-app-pub-4384579203907671/1457581004";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        isDebug = Debug.isDebugBuild;
    }

    private void Start()
    {
        MobileAds.Initialize("ca-app-pub-4384579203907671~2770662670");
        
        if(mRewardBasedVideoAd == null) mRewardBasedVideoAd = RewardBasedVideoAd.Instance;
        mInterstitialAd = new InterstitialAd(isDebug ? TestAdUnitInterstitialId : InterstitialId);
        
        SubscribeRewardAdForRetry();
        SubscribeInterstitialAdEvents();
        
        if (Application.platform == RuntimePlatform.Android)
        {
            if(!mRewardBasedVideoAd.IsLoaded()) StartCoroutine(LoadRewardedAd());
            if(!mInterstitialAd.IsLoaded()) StartCoroutine(LoadInterstitialAd());
        } else if (Application.isEditor)
        {
            canLoadAd = true;
            interstitialAdCanLoad = true;
        }
    }

    public void CleanRewardedAdEvents()
    {
        mRewardBasedVideoAd.OnAdLoaded -= OnAdLoaded;
        mRewardBasedVideoAd.OnAdFailedToLoad -= OnAdFailedToLoad;
        mRewardBasedVideoAd.OnAdOpening -= OnAdOpening;
        mRewardBasedVideoAd.OnAdStarted -= OnAdStarted;
        mRewardBasedVideoAd.OnAdRewarded -= OnAdRewarded;
        mRewardBasedVideoAd.OnAdRewarded -= OnAdRewardedForCoins;
        mRewardBasedVideoAd.OnAdClosed -= OnAdClosedForCoins;
        mRewardBasedVideoAd.OnAdClosed -= OnAdClosed;
        mRewardBasedVideoAd.OnAdLeavingApplication -= OnAdLeavingApp;
        rewardEventsSubscribed = false;
    }

    private void CleanInterstitialAdEvents()
    {
        mInterstitialAd.OnAdLoaded -= HandleOnAdLoaded;
        mInterstitialAd.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        mInterstitialAd.OnAdOpening -= HandleOnAdOpened;
        mInterstitialAd.OnAdClosed -= HandleOnAdClosed;
        mInterstitialAd.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
        interstitialAdEventsSubscribed = false;
    }
    private void SubscribeInterstitialAdEvents()
    {
        CleanInterstitialAdEvents();
        mInterstitialAd.OnAdLoaded += HandleOnAdLoaded;
        mInterstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        mInterstitialAd.OnAdOpening += HandleOnAdOpened;
        mInterstitialAd.OnAdClosed += HandleOnAdClosed;
        mInterstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        interstitialAdEventsSubscribed = true;
    }
    public void SubscribeRewardAdForRetry()
    {
        CleanRewardedAdEvents();
        retryRewarded = false;
        mRewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        mRewardBasedVideoAd.OnAdFailedToLoad += OnAdFailedToLoad;
        mRewardBasedVideoAd.OnAdOpening += OnAdOpening;
        mRewardBasedVideoAd.OnAdStarted += OnAdStarted;
        mRewardBasedVideoAd.OnAdRewarded += OnAdRewarded;
        mRewardBasedVideoAd.OnAdClosed += OnAdClosed;
        mRewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApp;
        rewardEventsSubscribed = true;
    }
    #region RewardedAdFunctions

    private IEnumerator LoadRewardedAd()
    {
        mRewardBasedVideoAd.LoadAd(new AdRequest.Builder().AddTestDevice("82F0C128E4B548A6A3B06D8F9A3845BD").Build(), isDebug ? TestAdUnitId : RewardedAdId);
        yield return null;
    } 
    public void ShowRewardedAd(RewardType type = RewardType.Attempt)
    {
        rewardType = type;
        if (Application.platform == RuntimePlatform.Android)
        {
            if (mRewardBasedVideoAd.IsLoaded())
            {
                mRewardBasedVideoAd.Show();
            }
        } else if (Application.isEditor)
        {
            if (rewardType == RewardType.Attempt)
            {
                GameController.Instance.StartCoroutine(GameController.Instance.Retry());
            }
            else
            {
                GameController.Instance.StartCoroutine(GameController.Instance.RewardCoins());
            }
        }
    }
    public void ChangeReward()
    {
        mRewardBasedVideoAd.OnAdClosed -= OnAdClosed;
        mRewardBasedVideoAd.OnAdRewarded -= OnAdRewarded;
        mRewardBasedVideoAd.OnAdRewarded += OnAdRewardedForCoins;
        mRewardBasedVideoAd.OnAdClosed += OnAdClosedForCoins;
    }
    private void ReloadRewardedAd()
    {
        if (Application.isEditor) canLoadAd = true;
        StartCoroutine(LoadRewardedAd());
        if (GameController.Instance.gameMode == GameMode.Adventure) 
            SubscribeToRewardCoins();
        else
            SubscribeRewardAdForRetry();
    }
    private IEnumerator UnSubscribeAdHandlers()
    {
        canLoadAd = false;
        CleanRewardedAdEvents();
        ReloadRewardedAd();
        yield return null;
    }
    private void SubscribeToRewardCoins()
    {
        mRewardBasedVideoAd.OnAdLoaded += OnAdLoaded;
        mRewardBasedVideoAd.OnAdFailedToLoad += OnAdFailedToLoad;
        mRewardBasedVideoAd.OnAdOpening += OnAdOpening;
        mRewardBasedVideoAd.OnAdStarted += OnAdStarted;
        mRewardBasedVideoAd.OnAdRewarded += OnAdRewardedForCoins;
        mRewardBasedVideoAd.OnAdClosed += OnAdClosedForCoins;
        mRewardBasedVideoAd.OnAdLeavingApplication += OnAdLeavingApp;
    }
    #endregion
    #region InterstitialAdFunctions
    private IEnumerator LoadInterstitialAd()
    {
        var adRequest = new AdRequest.Builder().AddTestDevice("82F0C128E4B548A6A3B06D8F9A3845BD").Build();
        mInterstitialAd.LoadAd(adRequest);
        yield return null;
    }
    public void ShowInterstitialAd(String scene)
    {
        mSceneToLoad = scene;
        var flag = mInterstitialAd.IsLoaded();
        if (flag)
        {
            mInterstitialAd.Show();
        }
    }
    #endregion
    #region RewardedAdEvents
    private void OnAdLoaded(object sender, EventArgs args) => canLoadAd = true;
    private void OnAdOpening(object sender, EventArgs args) => Debug.Log("Ad opened");
    private void OnAdStarted(object sender, EventArgs args) => ShowToast("Ad started");
    private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        canLoadAd = false;
        ShowToast("Ad failed to load " + args.Message);
        UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(LoadRewardedAd()));
    }
    private void OnAdClosed(object sender, EventArgs args) => UnityMainThreadDispatcher.Instance().Enqueue(() =>
    {
        canLoadAd = false;
        if(retryRewarded)
        {
            GameController.Instance.StartCoroutine(GameController.Instance.Retry());
            StartCoroutine(UnSubscribeAdHandlers());
            return;
        } 
        GameController.Instance.SetGameState(GameState.Ended);
    });
    private void OnAdClosedForCoins(object sender, EventArgs args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            StartCoroutine(ChangeToRetryRewardEvents());
        });
    }

    private void OnAdRewarded(object sender, Reward args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            retryRewarded = true;
        });
    }

    private void OnAdRewardedForCoins(object sender, Reward args)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameController.Instance.StartCoroutine(GameController.Instance.RewardCoins());
        });
    }

    private void OnAdLeavingApp(object sender, EventArgs args) => ShowToast("Application closed");
    
    #endregion
    #region InterstitialAdEvents
    private void HandleOnAdLoaded(object sender, EventArgs args) => interstitialAdCanLoad = true;
    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        interstitialAdCanLoad = false;
        UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(LoadInterstitialAd()));
    }
    private void HandleOnAdOpened(object sender, EventArgs args) => Debug.Log("HandleAdOpened event received");
    private void HandleOnAdClosed(object sender, EventArgs args) => UnityMainThreadDispatcher.Instance().Enqueue(() =>
    {
        interstitialAdCanLoad = false;
        StartCoroutine(LoadScene(mSceneToLoad));
        StartCoroutine(RenewInterstitialAd());
    });
    private void HandleOnAdLeavingApplication(object sender, EventArgs args) => Debug.Log("HandleAdLeavingApplication event received");
    #endregion

    private IEnumerator ChangeToRetryRewardEvents()
    {
        CleanRewardedAdEvents();
        StartCoroutine(LoadRewardedAd());
        SubscribeRewardAdForRetry();
        yield return null;
    }

    private IEnumerator RenewInterstitialAd()
    {
        mInterstitialAd.Destroy();
        mInterstitialAd = new InterstitialAd(isDebug ? TestAdUnitInterstitialId : InterstitialId);
        SubscribeInterstitialAdEvents();
        StartCoroutine(LoadInterstitialAd());
        yield return null;
    }
    private IEnumerator LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
        yield return null;
    }
    private static void ShowToast(string message = "") => Debug.Log(message);
}*/