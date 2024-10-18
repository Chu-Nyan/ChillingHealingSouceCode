using System;
using UnityEngine;

public enum ClickMode { Player, Housing }

public class ClickModeChanger : MonoBehaviour
{
    private IClickHandler[] _modes;
    private ClickMode _currentMode;

    public ClickMode CurrentMode
    {
        get => _currentMode;
    }

    public void Awake()
    {
        _modes = new IClickHandler[]
        {
            gameObject.AddComponent<PlayerClickHandler>(),
            gameObject.AddComponent< HousingClickHandler>()
        };
    }

    public void Init(Player player, Blueprint blueprint, CameraController camera)
    {
        if (_modes[(int)ClickMode.Player] is PlayerClickHandler click)
        {
            click.Init(player, camera);
        }
        if (_modes[(int)ClickMode.Housing] is HousingClickHandler housing)
        {
            housing.Init(player, blueprint);
        }
    }

    public void RegisterBehaviorEvent(Action<MissionType, int, int> action)
    {
        foreach (var mode in _modes)
        {
            mode.BehaviorEvent += action;
        }
    }

    public T ChangeMode<T>(ClickMode mode) where T : IClickHandler
    {
        _modes[(int)_currentMode].Unregister();
        _modes[(int)mode].Register();
        _currentMode = mode;
        return (T)_modes[(int)_currentMode];
    }

    public void ChangeMode(ClickMode mode)
    {
        _modes[(int)_currentMode].Unregister();
        _modes[(int)mode].Register();
        _currentMode = mode;
    }
}
