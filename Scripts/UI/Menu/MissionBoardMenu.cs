using UnityEngine;

public class MissionBoardMenu : MenuBase
{
    #region defaultValue

    private static readonly string _title = "미션 보드";
    private static readonly Vector2 _gridCellSize = new(300, 300);
    private static readonly TextAnchor _anchor = TextAnchor.UpperLeft;
    private static readonly Vector2 _spacing = new(10, 10);
    
    #endregion
    private MissionBoard _missionBoard;

    private void Awake()
    {
        _missionBoard = _UIM._missionBoard;
    }

    public override void Init()
    {
        Title = _title;
        Tabs = new string[] { };
        GridCellSize = _gridCellSize;
        Anchor = _anchor;
        Spacing = _spacing;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        foreach (EventMission mission in _missionBoard.EventMissions)
        {
            if (mission.State == MissionState.CanStart)
            {
                var slot = _UIM.GetPooledUI<MissionBoardSlotUI>(PooledUIName.MissionBoardSlotUI);
                slot.Init(mission);
                slot.Button.onClick.AddListener(() => AcceptMission(slot, mission.Orderer));
                slot.transform.SetParent(slotParent.gameObject.transform, false);
                slot.transform.SetAsLastSibling();
                Slots.Add(slot);
            }
        }
    }

    private void AcceptMission(MissionBoardSlotUI slot, UnitType orderer)
    {
        var dialog = _UIM.GetUI<MissionAcceptDialogueUI>(UIName.MissionAcceptDialogUI);
        dialog.Init(slot.EventMission.AcceptDialog, orderer);
        dialog.Button.onClick.AddListener(() => CloseAcceptMission(dialog));
        dialog.gameObject.transform.SetAsLastSibling();
        
        _missionBoard.Contract(slot.EventMission);
        OnTabChange?.Invoke(null);
    }

    private void CloseAcceptMission(MissionAcceptDialogueUI dialogUI)
    {
        dialogUI.Deactivate();
    }
}
