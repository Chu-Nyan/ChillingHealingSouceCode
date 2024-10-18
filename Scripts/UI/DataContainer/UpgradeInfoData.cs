public struct UpgradeInfoData
{
    public int Level;
    public CoinType CoinType;
    public int RequestItem;
    public int Value;

    public UpgradeInfoData(int level, CoinType coin, int requestItem, int value)
    {
        Level = level;
        CoinType = coin;
        RequestItem = requestItem;
        Value = value;
    }
}
