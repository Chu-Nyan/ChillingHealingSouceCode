using System.Collections.Generic;
using UnityEngine;

public class GroundNode : IPrioritizable
{
    public PathFindingData<GroundNode> PathfindingData;
    private GroundNodeLocalStatus _localStatus;
    private List<Camping> _campingEvent;

    public Vector2Int Position 
    { 
        get => _localStatus.Position; 
    }

    public Vector2Int WorldPosition
    { 
        get => _localStatus.WorldPosition; 
    }

    public Camping Interaction 
    { 
        get => _campingEvent[_campingEvent.Count - 1]; 
    }

    public bool CanInteraction
    {
        get => _localStatus.CanInteraction;
    }

    public bool IsBlocked
    {
        get => _localStatus.IsBlocked;
        set => _localStatus.IsBlocked = value;
    }

    public bool WasGroundObject
    {
        get => _localStatus.WasGroundObject;
        set => _localStatus.WasGroundObject = value;
    }

    public int Priority
    {
        get => PathfindingData.F;
    }

    public int QueueIndex
    {
        get => PathfindingData.QueueIndex;
        set => PathfindingData.QueueIndex = value;
    }

    public GroundNode(Vector2Int position, Vector2Int wolrdPosition)
    {
        _localStatus = new();
        _localStatus.Position = position;
        _localStatus.WorldPosition = wolrdPosition;
    }

    public void ResetPriorityData()
    {
        PathfindingData = new();
    }

    public void RegisterInteraction(Camping obj)
    {
        if (_campingEvent == null)
            _campingEvent = new();

        _campingEvent.Add(obj);
        _localStatus.CanInteraction = true;
    }

    public void UnregisterInteraction(Camping obj)
    {
        _campingEvent.Remove(obj);
        if (_campingEvent.Count == 0)
        {
            _localStatus.CanInteraction = false;
        }
    }
}
