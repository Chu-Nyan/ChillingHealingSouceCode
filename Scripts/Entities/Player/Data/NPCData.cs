using System;
using System.Collections.Generic;

[Serializable]
public class NPCData : IUpgradeable
{
    public static readonly int MaxLevel = 40;
    public static readonly int Speed = 5;
    public static List<LevelData> LevelData;
    public UnitType Type;
    public string Name;
    public string Desc;
    public string SkillName;

    public int Level = 0;

    public LevelData CurrentLevel
    {
        get => LevelData[Level];
    }

    public bool IsUpgradeable
    {
        get => Level < MaxLevel;
    }

    public CollectionUIData CollectionUIData
    {
        get
        {
            var data = LevelData[Level];
            return new(UpgradeType.NPC, Name, $"{SkillName} Lv.", "업그레이드", Desc, Level, data.Coin, data.CoinType, data.NextLevelPrice);
        }
    }

    public bool TryGetLevelValue(int level, out LevelData data)
    {
        data = default;
        bool result = LevelData[level] != null;
        if (result == true)
        {
            data = LevelData[level];
        }

        return result;
    }

    public void Upgrade()
    {
        if (IsUpgradeable == false)
            return;

        Level++;
    }

    public LevelData GetCurrentLevelData()
    {
        return CurrentLevel;
    }
}
