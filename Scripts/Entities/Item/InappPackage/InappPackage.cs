using System.Collections.Generic;

public class InappPackage
{
    public PackageType Type;
    public bool IsConsumeable;
    public int HealingCoin;
    public int MissionCoin;
    public List<RewardItem> RewardItems = new();
    public string SpritePath;
    public string Name;
    public string Price;
    public int BonusHealingCoin;

    private bool _hasReceipt;

    public bool HasReceipt
    {
        get => _hasReceipt;
        set => _hasReceipt = value;
    }

    public void Provide(Player player)
    {
        player.Data.AddCoinAmount(CoinType.Mission, MissionCoin);
        if (_hasReceipt == false)
        {
            UnityEngine.Debug.Log("첫구매 보너스" + BonusHealingCoin);
            player.Data.AddCoinAmount(CoinType.Healing, HealingCoin + BonusHealingCoin);
            _hasReceipt = true;
        }
        else
        {
            player.Data.AddCoinAmount(CoinType.Healing, HealingCoin);
        }

        foreach (var item in RewardItems)
        {
            player.AddItem(item.Type, item.Amount);
        }
    }
}
