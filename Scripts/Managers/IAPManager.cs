using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviourSingleton<IAPManager>, IStoreListener
{
    private const string _environment = "production";

    private IStoreController _controller;
    private Dictionary<string, InappPackage> _inappPackageDB;
    private List<InappPackage> _shopPackage;
    private Player _player;
    private event Action<bool, PackageType> IsPurchaseSucessed;

    public readonly PackageType[] ShopCoinPackages = new PackageType[]
    {
        PackageType.coin1, PackageType.coin2, PackageType.coin3, PackageType.coin4, PackageType.coin5, PackageType.coin6
    };

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        LoadInappPackageData();
    }

    private void Initialize()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(_environment);

            UnityServices.InitializeAsync(options).ContinueWith(task => Debug.Log("Success"));
        }
        catch (Exception exception)
        {
            Debug.Log($"error Awake : {exception.Message}");
        }
    }



    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var item in _inappPackageDB)
        {
            if (item.Value.IsConsumeable == true)
            {
                builder.AddProduct(item.Value.Type.ToString(), ProductType.NonConsumable);
            }
            else
            {
                builder.AddProduct(item.Value.Type.ToString(), ProductType.Consumable);
            }
        }
        UnityPurchasing.Initialize(this, builder);

    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void BuyProduct(PackageType type)
    {
        if (_controller != null)
        {
            var text = type.ToString();
            if (_inappPackageDB[text].IsConsumeable == true && HasReceipt(type) == true)
                return;

            _controller.InitiatePurchase(text);
        }
        else
        {
            Debug.LogError("Controller is not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var id = purchaseEvent.purchasedProduct.definition.id;
        _inappPackageDB[id].Provide(_player);
        OnPurchaseSucessed(true, _inappPackageDB[id].Type);
        return PurchaseProcessingResult.Complete;
    }

    public bool HasReceipt(PackageType id)
    {
        var product = _controller.products.WithID(id.ToString());
        return product.hasReceipt;
    }

    public void RegisterPurchase(Action<bool, PackageType> action)
    {
        IsPurchaseSucessed += action;
    }

    public InappPackage GetPackage(PackageType type)
    {
        return _inappPackageDB[type.ToString()];
    }

    public List<InappPackage> GetShopPackageList()
    {
        var package = new List<InappPackage>();
        for (int i = 0; i <= 15; i++)
        {
            package.Add(_shopPackage[i]);
        }
        return package;
    }

    public Product GetProduct(PackageType type)
    {
        return _controller.products.WithID(type.ToString());
    }

    private void OnPurchaseSucessed(bool isSucessed, PackageType type)
    {
        IsPurchaseSucessed?.Invoke(isSucessed, type);
    }

    private void LoadInappPackageData()
    {
        _shopPackage = new();
        _inappPackageDB = AssetManager.Instance.DeserializeJsonSync<Dictionary<string, InappPackage>>(Const.Json_InappPackageDB);
        var rewardItemHelpers = AssetManager.Instance.DeserializeJsonSync<List<RewardItemHelper>>(Const.Json_InappPackageRewardDB);
        for (int i = 0; i < rewardItemHelpers.Count; i++)
        {
            var package = _inappPackageDB[rewardItemHelpers[i].Type.ToString()];
            package.RewardItems.Add(new RewardItem()
            {
                Type = rewardItemHelpers[i].ItemType,
                Amount = rewardItemHelpers[i].Amount,
            });
        }
        foreach (var item in _inappPackageDB)
        {
            _shopPackage.Add(item.Value);
        }
    }

    private void CheakAllPackageReceipt()
    {
        foreach (var item in _inappPackageDB)
        {
            item.Value.HasReceipt = HasReceipt(item.Value.Type);
        }
    }

    #region 인터페이스
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        CheakAllPackageReceipt();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("초기화 실패 : " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("초기화 실패 : " + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        var pack = _inappPackageDB[product.definition.id];
        OnPurchaseSucessed(false, pack.Type);
        Debug.Log("구매 실패");
    }
    #endregion
}
