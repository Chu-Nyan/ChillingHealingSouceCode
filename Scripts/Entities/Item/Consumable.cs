public class Consumable : ItemBase
{
    public static readonly ItemActionHandler handler = new();
    private UseEffect _effect;

    public UseEffect Effect 
    { 
        get => _effect; 
    }

    public void Init(ItemUniversalStatus universal, UseEffect effect)
    {
        _universalStatus = universal;
        _effect = effect;
    }

    public int Use(Player player, Map handler, int amount)
    {
        if (_effect.EffectType == UseEffectType.None)
            return amount;

        if (_effect.EffectType >= UseEffectType.MapResources)
            amount = UseMap(handler, amount);
        else // Player에 영향 주는 이펙트
            amount = UsePlayer(player, amount);

        Stack -= amount;
        return amount;
    }

    private int UsePlayer(Player player, int amount)
    {
        int before = amount;
        if (player != null)
        {
            amount -= handler.PlayerAction[_effect.EffectType].Invoke(player, _effect, amount);
        }

        return before - amount;
    }

    private int UseMap(Map map, int amount)
    {
        if (map != null)
        {
            amount -= handler.MapAction[_effect.EffectType].Invoke(map, _effect, amount);
        }

        return amount;
    }
}
