using System;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
using UnityEngine;
using UnityEngine.Events;

//2.2
public class RewAdmobManager : MonoBehaviour
{
    public static RewAdmobManager THIS;
#if GOOGLE_MOBILE_ADS
   private RewardedAd rewardBasedVideo;
    private Action resultCallback;
    
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;

    private void Awake()
    {
        if (THIS == null)
            THIS = this;
        else if(THIS != this)
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

     public void Start()
        {
            RequestRewardBasedVideo();
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }
        
        

        private void RequestRewardBasedVideo()
        {
#if UNITY_ANDROID
            string adUnitId = InitScript.Instance.admobRewardedUIDAndroid;
#elif UNITY_IPHONE
            string adUnitId = InitScript.Instance.admobRewardedUIDIOS;
#else
            string adUnitId = "unexpected_platform";
#endif
            
                 // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                    Debug.Log("Rewarded ad failed to load.");
                    return;
                }

                Debug.Log("Rewarded ad loaded.");
                rewardBasedVideo = ad;

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Rewarded ad opening.");
                    OnAdOpeningEvent.Invoke();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad closed.");
                    OnAdClosedEvent.Invoke();
                    RequestRewardBasedVideo();
                };
                ad.OnAdImpressionRecorded += () =>
                {
                    Debug.Log("Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                    Debug.Log("Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.Log("Rewarded ad failed to show with error: " +
                              error.GetMessage());
                    RequestRewardBasedVideo();
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                    Debug.Log(msg);
                    RequestRewardBasedVideo();
                };
            });
        }

        public bool IsRewardedAdIsLoaded()
        {
            return rewardBasedVideo.CanShowAd();
        }

        public void ShowRewardedAd(Action resultCallback)
        {
       
            if (!IsRewardedAdIsLoaded())
            {
                Debug.Log("Rewarded ad is not ready yet.");
                RequestRewardBasedVideo();
            }
        
            if (IsRewardedAdIsLoaded())
            {
                rewardBasedVideo.Show((Reward reward) =>
                {
                    this.resultCallback = resultCallback;
                    resultCallback.Invoke();
                    Debug.Log("Rewarded ad granted a reward: " + reward.Amount);
                });
            }
        }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.LoadAdError);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        this.RequestRewardBasedVideo();

        if (LevelManager.THIS?.gameStatus == GameState.PreFailed)
        {
            LevelManager.THIS.gameStatus = GameState.GameOver;
        }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
        if (resultCallback != null) resultCallback();
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }
    #endif
}
