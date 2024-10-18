public class Furniture : ItemBase
{
    private FurnitureUniversalData _furnitureData;

    public FurnitureUniversalData FurnitureData
    {
        get => _furnitureData;
    }

    public void Init(ItemUniversalStatus universal, FurnitureUniversalData data)
    {
        _universalStatus = universal;
        _furnitureData = data;
    }
}
