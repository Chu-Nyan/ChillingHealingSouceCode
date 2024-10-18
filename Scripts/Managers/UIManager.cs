using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    // 전부 팀원 코드 입니다.

    private Dictionary<UIName, UIBase> _UIDictionary = new();
    private Dictionary<PooledUIName, ObjectPooling<UIBase>> _UIPools;

    private HUDChanger _hubUI = new();
    public Player _player;
    public MissionBoard _missionBoard;
    public MapController _mapController;
    public Shop _shop;

    private AssetManager _assetManager = AssetManager.Instance;

    public void Init(Player player, MissionBoard missionBoard, MapController mapController, Shop shop)
    {
        _player = player;
        _missionBoard = missionBoard;
        _mapController = mapController;
        _shop = shop;
        InitUIPools();
        _hubUI.Init(GetUI<ButtonsUI>(UIName.ButtonsUI), GetUI<PlayerInfoUI>(UIName.PlayerInfoUI),
            GetUI<HousingUI>(UIName.HousingUI), GetUI<CharacterCustomUI>(UIName.CharacterCustomUI));
    }

    private void InitUIPools()
    {
        _UIPools = new Dictionary<PooledUIName, ObjectPooling<UIBase>>(
            new List<KeyValuePair<PooledUIName, ObjectPooling<UIBase>>>
            {
                InitUIPool<MenuTabUI>(PooledUIName.MenuTabUI),
                InitUIPool<InventorySlotUI>(PooledUIName.InventorySlotUI),
                InitUIPool<MissionSlotUI>(PooledUIName.MissionSlotUI),
                InitUIPool<MissionBoardSlotUI>(PooledUIName.MissionBoardSlotUI),
                InitUIPool<CollectionSlotUI>(PooledUIName.CollectionSlotUI),
                InitUIPool<ShopSlotUI>(PooledUIName.ShopSlotUI),
                InitUIPool<ShopItemSlotUI>(PooledUIName.ShopItemSlotUI),
                InitUIPool<GambleSlotUI>(PooledUIName.GambleSlotUI),
                InitUIPool<RedDotUI>(PooledUIName.RedDotUI),
                InitUIPool<TempPackageUI>(PooledUIName.TempPackageUI),
                InitUIPool<WorldMapSlotUI>(PooledUIName.WorldMapSlotUI),
                InitUIPool<ShopPaidItemTapUI>(PooledUIName.ShopPaidItemTapUI)
            });
    }

    private KeyValuePair<PooledUIName, ObjectPooling<UIBase>> InitUIPool<T>(PooledUIName name) where T : UIBase
    {
        return new KeyValuePair<PooledUIName, ObjectPooling<UIBase>>(name,
            new ObjectPooling<UIBase>(() => _assetManager.GenerateLoadAssetSync<T>(name.ToString())));
    }

    public T GetUI<T>(UIName name) where T : UIBase
    {
        if (_UIDictionary.TryGetValue(name, out UIBase value) == false)
        {
            value = _assetManager.GenerateLoadAssetSync<T>(name.ToString());
            _UIDictionary.Add(name, value);
        }
        value.Activate();

        return (T)value;
    }

    public T GetPooledUI<T>(PooledUIName name) where T : UIBase
    {
        var resultUI = _UIPools[name].Dequeue();
        resultUI.OnDeactivation += i => _UIPools[name].Enqueue(i);
        resultUI = resultUI.GetComponent<UIBase>();
        resultUI.Activate();

        return (T)resultUI;
    }

    public void InitUI(HousingController controller, GameSetting gamesetting)
    {
        PlayerInfoUI playerInfoUI = Instance.GetUI<PlayerInfoUI>(UIName.PlayerInfoUI);
        PlayerStatusUI playerStatusUI = playerInfoUI.GetComponentInChildren<PlayerStatusUI>();
        CoinUI coinUI = playerInfoUI.GetComponentInChildren<CoinUI>();
        playerStatusUI.Init(_player);
        coinUI.Init(_player);

        var buttonsUI = Instance.GetUI<ButtonsUI>(UIName.ButtonsUI);
        buttonsUI.Init(controller, gamesetting);
        EmojiUI.Init();
    }

    public void ChangeHUDUIMode(HUDChanger.Mode mode)
    {
        _hubUI.ChangeMode(mode);
    }

    public static void ShowNotificationUI(string text, Sprite sprite = null, float time = 4)
    {
        Instance.GetUI<NotificationUI>(UIName.NotificationUI).EnqueueText(text, sprite, time);
    }
}
