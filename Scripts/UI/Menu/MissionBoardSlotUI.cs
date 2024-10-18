using UnityEngine;
using UnityEngine.UI;

public class MissionBoardSlotUI : UIBase
{
    [SerializeField] public Image NPCPortrait;
    [SerializeField] public Button Button;

    public EventMission EventMission;
    private UnitType _NPC;
    
    public void Init(EventMission eventMission)
    {
        EventMission = eventMission;
        _NPC = EventMission.Orderer;
        string npcName = $"{_NPC.ToString()}";
        NPCPortrait.sprite =
            AssetManager.Instance.LoadAssetSync<Sprite>($"{npcName}[{npcName}_{4 * (int)Direction.Left}]");
        OnDeactivation += Deactivation;
    }

    public void Deactivation(UIBase uiBase)
    {
        Button.onClick.RemoveAllListeners();
    }
}
