using System;

public interface IClickHandler
{
    public event Action<MissionType, int, int> BehaviorEvent;

    public ClickMode ClickMode { get; }

    public void Register();
    public void Unregister();
}
