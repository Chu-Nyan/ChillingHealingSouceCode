using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionBoard
{
    private static Dictionary<int, Mission> _missionData;
    private static readonly int StartEventID = 2000;
    private readonly List<Mission> _findHelper;
    private List<Mission> _missions;
    private readonly List<EventMission> _eventMissions; // 수락하기
    private readonly List<EventMission> _lockEventMission;

    public Player Player;
    private Inventory _inventory;

    public event Action MissionUnlocked;

    public List<Mission> Missions
    {
        get => _missions;
    }

    public List<EventMission> EventMissions
    {
        get => _eventMissions;
    }

    public List<EventMission> LockEventMissions
    {
        get => _lockEventMission;
    }

    public MissionBoard()
    {
        _missions = new();
        _eventMissions = new();
        _findHelper = new();
        _lockEventMission = new();
        LoadData();
        NPCGenerator.Instance.Generated += (i) =>
        {
            AddMissionScore(MissionType.UnlockNPC, (int)i.Data.Type, 1);
        };
        SaveManager.Instance._missionBoard = this;
        TimeManager.Instance.OnDayChanged += ResetDailyMission;
    }

    public void SetMissionInventory(Player player)
    {
        Player = player;
        _inventory = Player.Inventory;
    }

    public void RefreshAll()
    {
        _findHelper.Clear();
        FindMission(_missions);
        FindMission(_eventMissions);
        FindMission(_lockEventMission);

        foreach (var item in _findHelper)
        {
            RefreshCollectItemMission(item);
        }

        void FindMission<T>(List<T> target) where T : Mission
        {
            foreach (var item in target)
            {
                if (item.GetMissionType == MissionType.CollectItem)
                {
                    _findHelper.Add(item);
                }
            }
        }
    }

    public void Contract(Mission mission)
    {
        if (mission.State != MissionState.CanStart)
            return;

        mission.State = MissionState.InProgress;
        if (mission.GetMissionType == MissionType.CollectItem)
        {
            RefreshCollectItemMission(mission);
        }
    }

    public void Complete(Mission mission, Player player)
    {
        if (mission.State != MissionState.CanComplete)
            return;

        mission.State = MissionState.Completed;
        SwitchCompleteReward(mission, player);
        if (mission.GetMissionType == MissionType.CollectItem)
        {
            ProvideItemForComplete(mission);
        }

        if (mission.Rewards is not null)
        {
            foreach (var reward in mission.Rewards)
            {
                player.AddItem(reward.Type, reward.Amount);
            }
        }
    }

    private void SwitchCompleteReward(Mission mission, Player player)
    {
        if (mission.ID < StartEventID)
        {
            player.Data.MissionCoin += mission.RewardCoin;
            _missions.Remove(mission);
            _missions.Add(mission);
        }
        else // mission.ID >= StartEventID
        {
            player.Data.HealingCoin += mission.RewardCoin;
            _eventMissions.Remove(mission as EventMission);
            _eventMissions.Add(mission as EventMission);
        }
    }

    private void ProvideItemForComplete(Mission mission)
    {
        _inventory.Remove((ItemType)mission.GetTargetType, mission.RequireScore);
    }

    public void AddMissionScore(MissionType type, int targetType, int value)
    {
        var mission = FindMission(type, targetType);
        foreach (var item in mission)
        {
            item.AddScore(value);
        }
    }

    private List<Mission> FindMission(MissionType type, int targetType)
    {
        _findHelper.Clear();
        FindMission(_missions);
        FindMission(_eventMissions);
        FindMission(_lockEventMission);
        return _findHelper;

        void FindMission<T>(List<T> target) where T : Mission
        {
            foreach (var item in target)
            {
                if (item.State == MissionState.Completed)
                    continue;

                if (item.GetMissionType == type && (item.GetTargetType == targetType || targetType == -1 || item.GetTargetType == -1))
                {
                    _findHelper.Add(item);
                }
            }
        }
    }

    private void LoadData()
    {
        var json = AssetManager.Instance.MergeJsonsSync(Const.Json_Mission, Const.Json_EventMission);
        var converter = new DuoConverter<Mission, EventMission>("ID", StartEventID, 200000);
        _missionData = AssetManager.Instance.DeserializeJsonSync<Dictionary<int, Mission>>(json, converter);

        SaveManager saveManager = SaveManager.Instance;
        bool isSaveFileLoaded = saveManager.IsSaveFileLoaded;
        Dictionary<int, MissionSaveData> datas = isSaveFileLoaded ? saveManager._saveData.MissionDictionary : null;

        foreach (var item in _missionData)
        {
            MissionSaveData data = isSaveFileLoaded ? datas[item.Key] : null;
            var itemValue = item.Value;
            if (itemValue.ID < StartEventID)
            {
                itemValue.State = isSaveFileLoaded ? data.MissionState : MissionState.InProgress;
                itemValue.Score = isSaveFileLoaded ? data.Score : 0;
                _missions.Add(itemValue);
            }
            else
            {
                var eventMission = itemValue as EventMission;

                if (isSaveFileLoaded)
                {
                    eventMission.State = isSaveFileLoaded ? data.MissionState : MissionState.Private;
                    eventMission.Score = isSaveFileLoaded ? data.Score : 0;
                    eventMission.UnlockScore = isSaveFileLoaded ? data.UnlockScore : 0;
                }

                if (eventMission.State == MissionState.Private)
                    _lockEventMission.Add(eventMission);
                else
                    _eventMissions.Add(eventMission);
            }
        }

        if (isSaveFileLoaded && SaveManager.Instance.IsDayChanged())
            ResetDailyMission();

        foreach (var item in _lockEventMission)
        {
            item.Unlocked += ShowEvent;
        }
    }

    private void RefreshCollectItemMission(Mission mission)
    {
        int amount = _inventory.GetTotalAmount((ItemType)mission.GetTargetType);
        mission.AddScore(amount);
    }

    private void ShowEvent(EventMission mission)
    {
        _lockEventMission.Remove(mission);
        _eventMissions.Add(mission);
        MissionUnlocked?.Invoke();
    }

    public void ResetDailyMission()
    {
        Debug.Log("ResetDailyMission");
        foreach (var mis in _missions)
        {
            mis.Reset();
        }

        var orderedList = _missions.OrderBy(x => x.ID);
        _missions = orderedList.ToList();

        AddMissionScore(MissionType.CheckIn, -1, 1);
    }
}
