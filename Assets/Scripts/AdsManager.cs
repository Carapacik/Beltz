using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
#if UNITY_ANDROID
    private const string GameId = "4168350";
#else
    private const string GameId = "4168351";
#endif
    private void Start()
    {
        Advertisement.Initialize(GameId);
        Advertisement.AddListener(this);
        ShowBannerAd();
    }

    public static void PlayInterstitialAd()
    {
        if (Advertisement.IsReady("Interstitial_Android")) Advertisement.Show("Interstitial_Android");
    }

    private void ShowBannerAd()
    {
        if (Advertisement.IsReady("Banner_Android"))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show("Banner_Android");
        }
        else
        {
            StartCoroutine(RepeatShowBanner());
        }
    }

    private void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    private IEnumerator RepeatShowBanner()
    {
        yield return new WaitForSeconds(1);
        ShowBannerAd();
    }

    public void PlayRewardedAd()
    {
        if (Advertisement.IsReady("Rewarded_Android"))
            Advertisement.Show("Rewarded_Android");
        else
            Debug.Log("Rewarded is not ready");
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ads are ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log($"Error {message}");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Video started");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == "Rewarded_Android" && showResult == ShowResult.Finished) HintEasy.ShowHint();
    }
}