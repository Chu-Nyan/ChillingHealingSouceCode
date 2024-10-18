public interface IUpgradeable
{
    public bool IsUpgradeable { get; }
    public CollectionUIData CollectionUIData { get; }
    public bool TryGetLevelValue(int level, out LevelData data);
    public LevelData GetCurrentLevelData();
    public void Upgrade();
}
