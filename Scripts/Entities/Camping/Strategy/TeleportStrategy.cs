public class TeleportStrategy : ICampingStrategy
{
    private MapCategory _targetMap;

    public void Init(MapCategory category)
    {
        _targetMap = category;
    }

    public void Interact(Camping campingEvent, Player player)
    {
        player.MoveInsideAndOutside(_targetMap);
    }

    public void Interact(Camping campingEvent, NPC npc)
    {
        npc.UnitUI.ShowBubble("다음에 놀러 갈게!", true);
    }

    public void Leave(Camping campingEvent, Player player)
    {
    }

    public void Leave(Camping campingEvent, NPC npc)
    {
    }
}
