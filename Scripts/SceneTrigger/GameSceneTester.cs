using System.Collections.Generic;

public class GameSceneTester
{
    public void Item_Stack(Player player)
    {
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.DryShampoo, 20);
        player.AddItem(ItemType.Vita1004, 20);
        player.AddItem(ItemType.Vita1004, 20);
        player.AddItem(ItemType.Vita1004, 20);
        player.AddItem(ItemType.Vita1004, 20);
        player.AddItem(ItemType.Vita1004, 20);
    }

    public void Item_PlayerUse(Player player)
    {
        List<ItemBase> list = new()
         {
             ItemGenerator.Instance.Ready(ItemType.DryShampoo).SetStack(20).Generate(),
             ItemGenerator.Instance.Ready(ItemType.Vita1004).SetStack(20).Generate(),
         };

        player.Data.HP = 1;
        player.Data.Cleanliness = 1;

        foreach (var item in list)
        {
            var con = (Consumable)item;
            player.UseItem(con, 1);
        }
    }

    public void Shop_SellBuy(Player player, Shop shop)
    {
        shop.TryBuy(player, ItemType.AccessoryBox, 20);
        player.Data.HealingCoin = 10000;
        player.Data.MissionCoin = 10000;
        shop.TryBuy(player, ItemType.Vita1004, 20);
    }
}
