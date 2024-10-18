public class FurnitureUniversalData
{
    public ItemType Type;
    public int[] Size;
    public bool IsBlocked;
    public bool CanRotation;
    public string FrontSpritePath => $"Furniture{(int)Type}_0";
    public string SideSpritePath => $"Furniture{(int)Type}_1";
}
