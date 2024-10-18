using UnityEngine;

public class CampingUniversalData
{
    private static readonly LevelData UnlockData = new(0, CoinType.Mission, 0, 0);
    public static readonly int DefalutMaxLevel = 1;

    public CampingType Type;
    public InteractionType InteractionType;
    public string Name;
    public int[] Size;
    public bool IsBlocked;
    public bool IsProactive;

    public Vector2 SizeVector => new(Size[0], Size[1]);

    public virtual bool IsUpgradeable(int level)
    {
        return level < DefalutMaxLevel;
    }

    public virtual LevelData CurrentLevel(int level)
    {
        return UnlockData;
    }

    public virtual bool TryGetLevelValue(int level, out LevelData infoData)
    {
        infoData = UnlockData;
        return level == 0;
    }
}