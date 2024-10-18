using GoogleMobileAds.Api;
using System;
using System.Collections;

public class AdMobManager : MonoBehaviourSingleton<AdMobManager>
{
    // MyRewardADID = "sourcecode1";
    // AndroidThemeID = "sourcecode2";
    private const string _androidRewardId = "sourcecode1";

    public readonly DailyBuyItemData[] PaidAdItems = new DailyBuyItemData[]
    {
        new (0, ItemType.RandomMissionCoin0, 5),
        new (1, ItemType.HairBox, 3),
        new (2, ItemType.ClothingBox, 3),
        new (3, ItemType.FurnitureBox, 3)
    };

    private RewardedAd _rewardedAd;

    private void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
        });
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
    }

    public void ShowRewardedAd(Action resultAction, Action failed = null)
    {
        if (IAPManager.Instance.HasReceipt(PackageType.ad_remove) == true)
        {
            resultAction?.Invoke();
        }
        else
        {
            var ui = UIManager.Instance.GetUI<AsynLoadingUI>(UIName.AsynLoadingUI);
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(_androidRewardId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (ad == null || error != null)
                    {
                        failed?.Invoke();
                        return;
                    }

                    _rewardedAd = ad;
                    CompleteLoading(resultAction);
                    ui.Deactivate();
                });
        }
    }

    private void CompleteLoading(Action resultAction)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                resultAction?.Invoke();
            });
            DestroyAd();
        }
    }

    private IEnumerator DelayAction(Action action)
    {
        yield return null;
        action?.Invoke();
    }

    private void DestroyAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
    }
}
