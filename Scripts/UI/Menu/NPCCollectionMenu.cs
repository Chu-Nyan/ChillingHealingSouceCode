using UnityEngine;

public class NPCCollectionMenu : CollectionUI
{
    private static readonly string _title = "동물 친구들";
    public override void Init()
    {
        base.Init();
        Title = _title;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        if (currentTab > (int)MapType.Sea)
            return;

        var npc = _mapController.MapDatas[(MapType)currentTab].Npcs;
        var campings = _mapController.MapDatas[(MapType)currentTab].Campings;

        for (int i = 0; i < npc.Count; i++)
        {
            var slot = _UIM.GetPooledUI<CollectionSlotUI>(PooledUIName.CollectionSlotUI);
            slot.transform.SetParent(slotParent.transform, false);
            slot.transform.SetAsLastSibling();
            Slots.Add(slot);
            if (npc[i].Level == 0)
            {
                slot.SetUnlockImage(false);
            }
            else
            {
                slot.SetUnlockImage(true);
                var sprite = NPCGenerator.Instance.GetSprite(npc[i].Type);
                SetSlot(slot, npc[i], sprite);
            }
        }
    }
}
