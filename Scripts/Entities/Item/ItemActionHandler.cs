using System;
using System.Collections.Generic;
using Unity.Mathematics;

public class ItemActionHandler
{
    public static readonly List<ItemType> ItemarrayHelper = new List<ItemType>(8);
    public readonly Dictionary<UseEffectType, Func<Map, UseEffect, int, int>> MapAction = new();
    public readonly Dictionary<UseEffectType, Func<Player, UseEffect, int, int>> PlayerAction = new();
    public readonly Dictionary<ItemType, GambleProbabilityData> GambleProbability;

    public ItemActionHandler()
    {
        MapAction = new()
        {
            { UseEffectType.MapResources, AddCurrentMapResources },
            { UseEffectType.MapPollution, ReduceCurrentMapPollution },
        };

        PlayerAction = new()
        {
            { UseEffectType.HP, AddHP },
            { UseEffectType.CleanLiness, AddCleanliess },
            { UseEffectType.RandomItem, RandomBox },
            { UseEffectType.Upgrade, UpgradeCampingCar},
            { UseEffectType.FourLeafCloverConversion, ConvertFourLeafClover},
            { UseEffectType.AdSkip, AdSkip},
            { UseEffectType.RandomHealingCoin, RandomHealingCoinSmall},
            { UseEffectType.RandomHealingCoin2, RandomHealingCoinMedium},
            { UseEffectType.GetHealingCoin, AddHealingCoin},
            { UseEffectType.ExpandInventory, RareRandomBax},
            { UseEffectType.RareRandomItem, RareRandomBax}
        };
        GambleProbability = LoadGambleProbability();
    }

    public void Use(UseEffect effect, Player player, Map map, int amount)
    {
        if (PlayerAction.ContainsKey(effect.EffectType) == true)
        {
            PlayerAction[effect.EffectType]?.Invoke(player, effect, amount);
        }
        else
        {
            MapAction[effect.EffectType]?.Invoke(map, effect, amount);
        }
    }

    // Return UseItemCount;
    private int AddHP(Player player, UseEffect effect, int amount)
    {
        amount = GetRealityUsageAmount(effect, amount);
        if (PlayerData.MaxHP != player.Data.HP)
        {
            int loop = UseForFull(PlayerData.MaxHP, player.Data.HP, effect.Value);
            amount = math.min(amount, loop);
            player.Data.HP += effect.Value * amount;
        }

        return amount * effect.UsageAmount;
    }

    private int AddCleanliess(Player player, UseEffect effect, int amount)
    {
        amount = GetRealityUsageAmount(effect, amount);
        if (PlayerData.MaxCleanliness != player.Data.Cleanliness)
        {
            int loop = UseForFull(PlayerData.MaxCleanliness, player.Data.Cleanliness, effect.Value);
            amount = math.min(amount, loop);
            player.Data.Cleanliness += effect.Value * amount;
        }

        return amount * effect.UsageAmount;
    }

    private int AdSkip(Player player, UseEffect effect, int amount)
    {
        return amount;
    }

    private int RandomHealingCoinSmall(Player player, UseEffect effect, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int value = UnityEngine.Random.Range(0, 10);
            if (value == 0)
            {
                value = 1;
            }
            else
            {
                value *= 50;
            }

            UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI)
                .EnqueueText($"힐링코인 {value}개를 획득하였습니다.");
            player.Data.AddCoinAmount(CoinType.Healing, value);
        }

        return amount;
    }

    private int RandomHealingCoinMedium(Player player, UseEffect effect, int amount)
    {
        int value = UnityEngine.Random.Range(0, 30);
        if (value == 0)
        {
            value = 1;
        }
        else
        {
            value *= 1000;
        }

        UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI)
            .EnqueueText($"힐링코인 {value}개를 획득하였습니다.");
        player.Data.AddCoinAmount(CoinType.Healing, value);

        return amount;
    }

    private int ExpandInventory(Player player, UseEffect effect, int amount)
    {
        var minSpace = player.Inventory.Space.GetMinCategorySpace;
        if (minSpace >= 999)
        {
            UIManager.ShowNotificationUI("더 이상 확장할 수 없습니다.");
            return 0;
        }
        else
        {
            int reality = (int)((999 - minSpace) / effect.Value) + 1;
            UnityEngine.Debug.Log($"{reality} , {amount}");
            amount = math.min(reality, amount);
            player.Inventory.Space.AddBounsSpace((int)effect.Value * amount);
            return amount;
        }
    }

    private int RandomBox(Player player, UseEffect effect, int amount)
    {
        ItemarrayHelper.Clear();
        var category = GambleProbability[effect.Type].Category;
        int reality = 0;
        var space = player.Inventory.Space;
        for (int i = 0; i < amount; i++)
        {
            if (space.GetMaxSpace(category) - space.ItemList[category].Count == 0)
                break;

            var realItem = Random(effect);
            ItemarrayHelper.Add(realItem);
            player.AddItem(realItem, 1);
            reality++;
        }

        if (reality != 0)
        {
            UIManager.Instance.GetUI<ShopGambleUI>(UIName.ShopGambleUI).Init(ItemarrayHelper);
            SoundManager.Instance.PlaySE(SoundType.ItemBoxOpening);
        }
        return reality;
    }


    private int RareRandomBax(Player player, UseEffect effect, int amount)
    {
        ItemarrayHelper.Clear();
        int reality = 0;
        ItemGenerator.Instance.TryGetConsumeablerDB(ItemType.RareRandomBox, out var successEffect);

        var space = player.Inventory.Space;
        for (int i = 0; i < amount; i++)
        {
            if (space.GetMaxSpace(ItemCategory.Furniture) - space.GetCurrentSpace(ItemCategory.Furniture) <= 0)
                break;

            bool isSuccess = UnityEngine.Random.Range(0, 1000) < 5;
            ItemType realItem;
            if (isSuccess == true)
            {
                realItem = Random(successEffect);
            }
            else
            {
                realItem = Random(effect);
            }

            ItemarrayHelper.Add(realItem);
            player.AddItem(realItem, 1);
            reality++;
        }

        if (reality != 0)
        {
            UIManager.Instance.GetUI<ShopGambleUI>(UIName.ShopGambleUI).Init(ItemarrayHelper);
            SoundManager.Instance.PlaySE(SoundType.ItemBoxOpening);
        }
        return reality;
    }

    private int AddCurrentMapResources(Map map, UseEffect effect, int amount)
    {
        amount = GetRealityUsageAmount(effect, amount);
        if (map.Resources != Map.MaxResources)
        {
            int loop = UseForFull(Map.MaxResources, map.Resources, effect.Value);
            amount = math.min(amount, loop);
            map.Resources += effect.Value;
        }

        return amount * effect.UsageAmount; ;
    }

    private int ConvertFourLeafClover(Player player, UseEffect effect, int amount)
    {
        amount = GetRealityUsageAmount(effect, amount);
        player.AddItem(ItemType.FourLeafClover, amount);
        return amount * effect.UsageAmount;
    }

    private int AddHealingCoin(Player player, UseEffect effect, int amount)
    {
        player.Data.AddCoinAmount(CoinType.Healing, (int)effect.Value * amount);
        return amount;
    }

    private int UpgradeCampingCar(Player player, UseEffect effect, int amount)
    {
        return player.UpgradeCampingCar() == true ? amount : 0;
    }

    private int ReduceCurrentMapPollution(Map map, UseEffect effect, int amount)
    {
        // pollution 관련 변수 추가 후 작성
        return amount;
    }

    private static int UseForFull(float max, float current, float addValue)
    {
        return (int)math.ceil((max - current) / addValue);
    }

    private int GetRealityUsageAmount(UseEffect effect, int Amount)
    {
        return Amount / effect.UsageAmount;
    }

    private Dictionary<ItemType, GambleProbabilityData> LoadGambleProbability()
    {
        var data = AssetManager.Instance.DeserializeJsonSync<List<GambleProbabilityDataHelper>>(Const.Json_GambleProbabilityDB);
        var newDic = new Dictionary<ItemType, GambleProbabilityData>();
        foreach (var item in data)
        {
            if (newDic.TryGetValue(item.Type, out var gambleData) == false)
            {
                gambleData = new();
                gambleData.Type = item.Type;
                gambleData.TypeInBox = new(32);
                newDic.Add(item.Type, gambleData);
            }

            gambleData.TypeInBox.Add(item.TargetItem);
        }

        return newDic;
    }


    private ItemType Random(UseEffect effect)
    {
        int randomIdx = new System.Random().Next(GambleProbability[effect.Type].TypeInBox.Count);
        return GambleProbability[effect.Type].TypeInBox[randomIdx];
    }
}
