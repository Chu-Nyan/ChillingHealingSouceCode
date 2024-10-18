using System.Collections;
using UnityEngine;

public class ShopPaidItemTapUI : UIBase
{
    [SerializeField]
    private NamePackageSlot[] _iapPackages;
    [SerializeField]
    private NamePackageSlot[] _adPackages;

    private PackageType[] _namePackages;
    private DailyBuyItemData[] _adBuyItemData;
    private Player _player;

    public void Init(Player player, PackageType[] namePackages, DailyBuyItemData[] adPackage)
    {
        _player = player;
        _namePackages = namePackages;
        _adBuyItemData = adPackage;
        Refresh();
    }

    // 팀원 코드
    public void Refresh()
    {
        for (int i = 0; i < _iapPackages.Length; i++)
        {
            if (i < _namePackages.Length)
            {
                var data = IAPManager.Instance.GetPackage(_namePackages[i]);
                var sprite = AssetManager.Instance.LoadAssetSync<Sprite>($"{Const.Sprite_Package}{(int)_namePackages[i]}");
                _iapPackages[i].Refresh(i, data.Name, sprite, ClickIAPPackage);
                _iapPackages[i].RefreshFirstPurchase(data.HasReceipt == false, $"첫구매\n + {Const.TextEmoji_HealingCoin} {data.BonusHealingCoin}");
                _iapPackages[i].RefreshInfo($"{data.Price}");
            }
        }

        for (int i = 0; i < _adPackages.Length; i++)
        {
            if (i < _namePackages.Length)
            {
                var data = _adBuyItemData[i];
                var itemDB = ItemGenerator.Instance.GetDB(data.Type);
                var sprite = AssetManager.GetItemIconSprite(data.Type);

                _adPackages[i].Refresh(i, itemDB.Name, sprite, ClickADPackage);
                _adPackages[i].RefreshInfo($"남은 횟수 : {data.MaxCount - data.Count}");
            }
        }
    }


    private void ClickIAPPackage(int index)
    {
        IAPManager.Instance.BuyProduct(_namePackages[index]);
    }

    private void ClickADPackage(int index)
    {
        var reward = _adBuyItemData[index];
        if (reward.CanBuy() == true)
            AdMobManager.Instance.ShowRewardedAd(GetReward);
        else
            UIManager.ShowNotificationUI("사용 가능한 모든 횟수를 사용하였습니다.");

        void GetReward()
        {
            reward.Count++;
            Refresh();

            if (ItemGenerator.Instance.TryGetConsumeablerDB(reward.Type, out var data) == true
             && data.IsInstance == true)
                _player.InstanceUseItem(reward.Type, 1);
            else
                _player.AddItem(reward.Type, 1);
        }
    }
}
