using System;
using System.Collections.Generic;

[Serializable]
public class ActiveCampingData : CampingUniversalData
{
    private readonly static int MinLevel = 0;
    private readonly static int MaxLevel = 20;
    public List<LevelData> LevelData;
    public UnitType NPC;
    public int DecreaseHp;
    public int DecreaseCleanliness;
    public SoundType SoundPath;
    public ItemType MissionItem;
    public EmojiUI.EmojiType EmojiType;

    public override bool IsUpgradeable(int level)
    {
        return level < MaxLevel;
    }

    public override LevelData CurrentLevel(int level)
    {
        return LevelData[level];
    }

    public override bool TryGetLevelValue(int level, out LevelData infoData)
    {
        bool result = level < LevelData.Count;
        level = Math.Clamp(level, MinLevel, MaxLevel);
        infoData = LevelData[level];

        return result;
    }
}

