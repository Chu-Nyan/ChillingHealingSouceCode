public interface ICampingStrategy
{
    public void Interact(Camping campingEvent, Player player);
    public void Interact(Camping campingEvent, NPC npc);
    public void Leave(Camping campingEvent, Player player);
    public void Leave(Camping campingEvent, NPC npc);
}
