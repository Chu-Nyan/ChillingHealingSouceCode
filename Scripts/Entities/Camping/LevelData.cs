public class LevelData
{
    public int Level;
    public CoinType CoinType;
    public int Coin;
    public int NextLevelPrice;

    public LevelData() { }

    public LevelData(LevelDataHelper helper)
    {
        CoinType = helper.CoinType;
        Level = helper.Level;
        Coin = helper.Coin;
        NextLevelPrice = helper.NextLevelPrice;
    }

    public LevelData(int level, CoinType coinType, int coin, int levelPrice)
    {
        Level = level;
        CoinType = coinType;
        Coin = coin;
        NextLevelPrice = levelPrice;
    }
}
