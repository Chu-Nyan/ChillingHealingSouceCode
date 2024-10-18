using System;
using System.Collections.Generic;

public class Mission
{
    public int ID; // 미션 ID
    public string Title; // 미션 설명
    public MissionType MissionType; // 미션 종류
    public int TargetType;// 목표 타입
    public int RequireScore; // 미션 진행 횟수(목표 개수)
    public int RewardCoin;
    public List<RewardItem> Rewards; // TODO : 사라진 데이터 시간남으면 제거

    private MissionState _state;
    public int Score;

    public Action<int> OnScoreChanged;
    public Action<MissionState> OnStateChanged;

    public MissionState State
    {
        get => _state;
        set
        {
            if (value != _state)
            {
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }
    }

    public virtual MissionType GetMissionType
    {
        get => MissionType;
    }

    public virtual int GetTargetType
    {
        get => TargetType;
    }

    public virtual void AddScore(int value)
    {
        if (State < MissionState.InProgress)
            return;

        Score += value;
        OnScoreChanged?.Invoke(Score);
        if (Score >= RequireScore && State != MissionState.Completed)
        {
            State = MissionState.CanComplete;
        }
        else if (State == MissionState.CanComplete && Score < RequireScore)
        {
            State = MissionState.InProgress;
        }
    }

    public virtual void Reset()
    {
        Score = 0;
        _state = MissionState.InProgress;
    }
}
