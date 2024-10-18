using System.Collections.Generic;

public class GambleProbabilityData
{
    public ItemType Type;
    public List<ItemType> TypeInBox;

    public ItemCategory Category
    {
        get
        {
            if (TypeInBox == null || TypeInBox.Count == 0)
            {
                return ItemCategory.Equipment;
            }
            else
            {
                return ItemGenerator.SwitchTypeToItemCategory(TypeInBox[0]);
            }
        }
    }
}
