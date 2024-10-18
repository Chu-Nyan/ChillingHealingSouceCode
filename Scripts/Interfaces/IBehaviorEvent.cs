using System;

public interface IBehaviorEvent
{
    public event Action<MissionType, int, int> BehaviorEvent;
}
