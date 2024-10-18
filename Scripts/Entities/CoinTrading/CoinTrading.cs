using System;

public class CoinTrading
{
    public static readonly int ThreeToFourScale = -10;
    public static readonly int FourToHealingCoinScale = 10000;
    public static readonly int MissionCoinToHealingCoinScale = 240;
    public static readonly int MaxTradeCount = 240;

    private Player _player;
    private CoinTradingUI _ui;

    public void Init(Player palyer)
    {
        _player = palyer;
        _ui = UIManager.Instance.GetUI<CoinTradingUI>(UIName.CoinTradingUI);
        _ui.Init(this);
        _ui.Deactivate();
    }

    public int GetThreeToFourCount()
    {
        return (int)(_player.Inventory.GetTotalAmount(ItemType.ThreeLeafClover) * 0.1f);
    }

    public int GetFourCloverCount()
    {
        return _player.Inventory.GetTotalAmount(ItemType.FourLeafClover);
    }

    public int GetMissionCoinToHealingCoinCount()
    {
        return _player.Data.MissionCoin;
    }

    public void ChangeThreeToFourScale(int amount)
    {
        var realityRemove = _player.Inventory.Remove(ItemType.ThreeLeafClover, -(amount * ThreeToFourScale));
        var remain = realityRemove % 10;
        if (remain != 0)
        {
            _player.AddItem(ItemType.ThreeLeafClover, remain);
        }

        var addCount = (int)(realityRemove * 0.1f);
        _player.AddItem(ItemType.FourLeafClover, addCount);
    }

    public void ChangeFourToHealingCoin(int amount)
    {
        var realityRemove = _player.Inventory.Remove(ItemType.FourLeafClover, amount);
        _player.Data.AddCoinAmount(CoinType.Healing, realityRemove * FourToHealingCoinScale);
    }

    public void ChangeMissionCoinToHealingCoinScale(int amount)
    {
        var reality = Math.Min(_player.Data.MissionCoin, amount);
        _player.Data.AddCoinAmount(CoinType.Mission, -reality);
        _player.Data.AddCoinAmount(CoinType.Healing, reality * MissionCoinToHealingCoinScale);
    }
}
