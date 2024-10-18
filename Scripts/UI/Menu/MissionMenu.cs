using System.Collections.Generic;
using UnityEngine;

public class MissionMenu : MenuBase
{
    #region defaultValue

    private static readonly string _title = "미션";
    private static readonly Vector2 _gridCellSize = new(900, 250);
    private static readonly TextAnchor _anchor = TextAnchor.UpperCenter;

    public enum MissionTab
    {
        Daily,
        Event
    }

    public static readonly Dictionary<MissionTab, string> TabName = new()
    {
        { MissionTab.Daily, "일일" },
        { MissionTab.Event, "이벤트" }
    };

    #endregion
    
    private MissionBoard _missionBoard;

    private void Awake()
    {
        _missionBoard = _UIM._missionBoard;
    }

    public override void Init()
    {
        Title = _title;
        Tabs = new[] { TabName[MissionTab.Daily], TabName[MissionTab.Event] };
        GridCellSize = _gridCellSize;
        Anchor = _anchor;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        if (currentTab == 0) // daily mission
        {
            foreach (Mission mission in _missionBoard.Missions)
                SetSlot(slotParent, mission);
        }
        else // currentTab == 1, event mission
        {
            foreach (EventMission mission in _missionBoard.EventMissions)
            {
                if (mission.State >= MissionState.InProgress)
                    SetSlot(slotParent, mission);
            }
        }
    }

    private void SetSlot<T>(GameObject slotParent, T mission) where T : Mission
    {
        var slot = _UIM.GetPooledUI<MissionSlotUI>(PooledUIName.MissionSlotUI);
        slot.Init(mission);
        slot.transform.SetParent(slotParent.gameObject.transform, false);
        slot.transform.SetAsLastSibling();
        slot.RewardsBtn.onClick.AddListener(() => GetReward(mission));
        Slots.Add(slot);
    }

    private void GetReward(Mission mission)
    {
        _missionBoard.Complete(mission, _missionBoard.Player);
        OnTabChange?.Invoke(null);
    }
}
