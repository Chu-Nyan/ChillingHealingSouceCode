using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviourSingleton<TimeManager>
{
    public static readonly float HourScale = 60; // 인게임 1시간 : 현실 1분
    private int _day;
    private int _hour;
    private float _time;
    private DateTime _loginTime;
    private DateTime _midnight;

    private float _totalIngameTime = 0;
    private float _dayChangeTime = 0;
    private bool _isChanged = false;

    private HashSet<ITimeListener> _listener = new();
    private HashSet<ITimeListener> _hourListener = new();
    private HashSet<ITimeListener> _removeWating = new();

    public Action OnDayChanged;

    protected override void Awake()
    {
        base.Awake();
        _loginTime = DateTime.Now;
        SetDayChangeTime();
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        _time += delta;
        /* 팀원 코드
        _totalIngameTime += delta;
        if (_totalIngameTime > _dayChangeTime && _isChanged == false)
        {
            _isChanged = true;
            OnDayChanged?.Invoke();
        }
        */

        if (_time > HourScale)
        {
            _time = 0;
            AddHour();
            foreach (var item in _hourListener)
            {
                item.GetTime(HourScale);
            }
        }
        foreach (var item in _listener)
        {
            item.GetTime(delta);
        }

        if (_removeWating.Count > 0)
        {
            RemoveWaiting();
        }
    }

    public void RegisterListener(ITimeListener listener)
    {
        _listener.Add(listener);
    }

    public void RegisterHourListener(ITimeListener listener)
    {
        _hourListener.Add(listener);
    }

    public void UnregisterListener(ITimeListener listener)
    {
        _removeWating.Add(listener);
    }

    private void RemoveWaiting()
    {
        foreach (var item in _removeWating)
        {
            _listener.Remove(item);
            _hourListener.Remove(item);
        }
        _removeWating.Clear();
    }

    private void AddHour()
    {
        if (_hour == 23)
        {
            _day++;
            _hour = 0;
        }
        else
        {
            _hour++;
        }
    }

    // 팀원 코드
    private void SetDayChangeTime()
    {
        _midnight = new DateTime(_loginTime.Year, _loginTime.Month, _loginTime.Day + 1, 0, 0, 0);
        var timeDiff = _midnight - _loginTime;
        _dayChangeTime = (int)timeDiff.TotalSeconds;
        Debug.Log($"{_dayChangeTime} secs left for Reset Mission");
    }
}

public interface ITimeListener
{
    public void GetTime(float time);
}
