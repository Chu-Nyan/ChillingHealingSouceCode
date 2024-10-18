using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointEventRayCastHelper : Singleton<PointEventRayCastHelper>
{
    private PointerEventData _pointeEvent;
    private EventSystem _eventSystem;
    private List<RaycastResult> _result;

    public PointEventRayCastHelper()
    {
        _eventSystem = EventSystem.current;
        _pointeEvent = new(_eventSystem);
        _result = new();
    }

    public List<RaycastResult> RayCastAll(Vector3 ScreenPos)
    {
        _pointeEvent.position = ScreenPos;
        EventSystem.current.RaycastAll(_pointeEvent, _result);
        return _result;
    }
}