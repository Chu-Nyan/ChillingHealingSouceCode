using System;

public class EventMission : Mission
{
    public UnitType Orderer; // NPC ID
    public MissionType UnlockMissionType; // 미션 종류
    public int UnlockTargetType; // 목표 타입
    public int UnlockRequireScore; // 미션 진행 횟수(목표 개수)
    public string AcceptDialog; // 승낙 대화
    public string SucceedDialog; // 성공 대화

    public int UnlockScore;

    public event Action<EventMission> Unlocked;

    public override MissionType GetMissionType
    {
        get => State == MissionState.Private ? UnlockMissionType : MissionType;
    }    
    public override int GetTargetType
    {
        get => State == MissionState.Private ? UnlockTargetType : TargetType;
    }

    public override void AddScore(int value)
    {
        if (State == MissionState.Private)
        {
            UnlockScore += value;
            if (UnlockScore >= UnlockRequireScore)
            {
                State = MissionState.CanStart;
                Unlocked?.Invoke(this);
            }
        }
        else
        {
            base.AddScore(value);
        }
    }
}
