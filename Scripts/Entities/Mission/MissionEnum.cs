public enum MissionType
{
    VisitCamping = 10000, // 캠핑
    VisitCampingTogether = 10001,
    UnlockCamping,
    TalkNPC = 20000, // NPC
    UnlockNPC = 20001, // NPC
    CollectItem = 30000, // 아이템
    UseItem,
    CheckIn = 40000 // 기타
}

public enum ETCMissionType
{
    CheckIn
}

public enum MissionState
{
    Private,
    CanStart,
    InProgress,
    CanComplete,
    Completed
}
