public struct CollectionUIData
{
    public static CollectionUIData Error = default;

    public UpgradeType Type;
    public string TitleName;
    public string SkillName;
    public string UpgradeText;
    public string Desc;
    public int Level;
    public int Value;
    public CoinType CoinType;
    public int LevelPrice;

    public CollectionUIData(UpgradeType type, string title, string skill, string upgrade, string desc, int level, int value, CoinType coinType, int levelPrice)
    {
        Type = type;
        TitleName = title;
        SkillName = skill;
        UpgradeText = upgrade;
        Desc = desc;
        Level = level;
        Value = value;
        CoinType = coinType;
        LevelPrice = levelPrice;
    }
}
