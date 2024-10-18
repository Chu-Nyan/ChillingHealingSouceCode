using System;
using UnityEngine;

public class CampingData : IUpgradeable
{
    protected CampingUniversalData _universal;
    protected Rect _interactZone;
    public int _level;
    public Vector3 Position;

    public CampingType Type;
    public event Action<int> Upgraded; 

    public CampingUniversalData Universal
    {
        get => _universal;
        set => _universal = value;
    }

    public Rect InteractZone
    {
        get => _interactZone;
        set => _interactZone = value;
    }

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public bool IsUpgradeable
    {
        get => _universal.IsUpgradeable(_level);
    }

    public LevelData CurrentLevel
    {
        get => _universal.CurrentLevel(_level);
    }

    public CollectionUIData CollectionUIData
    {
        get
        {
            string upgradeText = _level == 0 ? "해금" : "업그레이드";
            _universal.TryGetLevelValue(_level, out var data);
            return new CollectionUIData(UpgradeType.Camping, Universal.Name, $"Lv. ", upgradeText, default, _level, data.Coin, data.CoinType, data.NextLevelPrice);
        }
    }
    
    public void Init()
    {
        Upgraded = null;
    }

    public bool TryGetLevelValue(int level, out LevelData infoData)
    {
        return _universal.TryGetLevelValue(level, out infoData);
    }

    public LevelData GetCurrentLevelData()
    {
        _universal.TryGetLevelValue(_level, out var data);
        return data;
    }

    public void Upgrade()
    {
        _level++;
        Upgraded?.Invoke(Level);
    }
}
