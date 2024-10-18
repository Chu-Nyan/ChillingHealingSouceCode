using System;
using System.Collections.Generic;

public class Shop
{
    // public enum ShopTab
    // {
    //     Character, CampingCar, Environment, Cash
    // }

    public static readonly float SellCorrection = 0.7f;

    private List<InappPackage> _packages;
    private Dictionary<ShopTab, ShopItemListData> _list;

    public DailyBuyItemData[] PaidAditems
    {
        get => AdMobManager.Instance.PaidAdItems;
    }    
    
    public PackageType[] PaidPackages
    {
        get => IAPManager.Instance.ShopCoinPackages;
    }

    public Dictionary<ShopTab, ShopItemListData> List
    {
        get => _list;
    }

    public void InitItemList()
    {
        _list = AssetManager.Instance.DeserializeJsonSync<Dictionary<ShopTab, ShopItemListData>>(Const.Json_ShopItemListDB);
        _packages = IAPManager.Instance.GetShopPackageList();

    }

    public bool TryBuy(Player player, ItemType type, int stack)
    {
        var db = ItemGenerator.Instance.GetDB(type);
        var coinAmount = player.Data.GetCoinAmount(db.CoinType);
        var actualItem = type;

        if (stack * db.Price <= coinAmount)
        {
            if (ItemGenerator.Instance.TryGetConsumeablerDB(type, out var consumeData) == true
             && consumeData.IsInstance == true)
            {
                player.InstanceUseItem(type, stack);
            }
            else
            {
                var remain = stack;
                stack -= player.AddItem(actualItem, stack);
            }
            player.Data.AddCoinAmount(db.CoinType, -(stack * db.Price));
        }
        else
        {
            UIManager.ShowNotificationUI("구매에 실패하였습니다.");
            return false;
        }

        return true;
    }

    public void TryBuyPackage(PackageType type)
    {
        IAPManager.Instance.BuyProduct(type);
    }

    public void Sell(Player player, ItemBase item, int stack)
    {
        var realityRemoveStack = Math.Min(item.Stack, stack);
        var addCoin = realityRemoveStack * item.UniversalStatus.Price * SellCorrection;
        item.Stack -= realityRemoveStack;
        player.Data.AddCoinAmount(item.UniversalStatus.CoinType, (int)addCoin);

        // TODO : 판매완료UI, realityRemoveStack 실제 제거된 스택, addCoin 추가한 코인 
    }
}
