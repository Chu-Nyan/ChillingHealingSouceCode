public class NPCCollection
{
    private CollectionStorage<UnitType> _collections;

    public NPCCollection()
    {
        _collections = new();
    }

    public void AddCollection(UnitType npc)
    {
        if (npc == UnitType.None)
            return;

        var collection = new Collection<UnitType>(npc, false);
        _collections.Add(collection);
    }

    public void Unlock(UnitType npc)
    {
        for (int i = 0; i < _collections.Count; i++)
        {
            if (_collections[i].Item != npc)
                continue;

            _collections[i].Unlock(true);
            break;
        }
    }

}
