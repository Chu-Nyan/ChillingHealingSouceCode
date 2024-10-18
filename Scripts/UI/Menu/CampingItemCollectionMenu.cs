using UnityEngine;

public class CampingItemCollectionMenu : CollectionUI
{
    private static readonly string _title = "캠핑 아이템";

    public override void Init()
    {
        base.Init();
        Title = _title;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        if (currentTab > (int)MapType.Sea)
            return;

        var campings = _mapController.MapDatas[(MapType)currentTab].Campings;
        var isUnlock = _mapController.MapDatas[(MapType)currentTab].IsUnlock;
        for (int i = 0; i < campings.Count; i++)
        {
            if (campings[i].Universal.Type >= CampingType.MissionBoard)
                continue;

            var slot = _UIM.GetPooledUI<CollectionSlotUI>(PooledUIName.CollectionSlotUI);
            slot.transform.SetParent(slotParent.transform, false);
            slot.transform.SetAsLastSibling();
            slot.SetUnlockImage(isUnlock);
            Slots.Add(slot);
            var sprite = CampingGenerator.Instance.GetSprite(campings[i].Universal.Type);

            SetSlot(slot, campings[i], sprite);
        }
    }
}
