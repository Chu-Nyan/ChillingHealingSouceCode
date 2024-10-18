using ChillingHealing.DataStructure;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : Singleton<Pathfinding>
{
    private const int MoveStarightCost = 10;
    private const int MoveDiagonalCost = 14;
    private const int Weight = 2;

    private readonly PriorityQueue<GroundNode> _openList;
    private readonly HashSet<GroundNode> _closeList;
    private GroundNode _current;
    private GroundNode _bestNode;
    private int _minH;

    public Pathfinding()
    {
        _openList = new PriorityQueue<GroundNode>(256);
        _closeList = new HashSet<GroundNode>(256);
    }

    public MethodResult Move(IUnit walker, float speed)
    {
        // TODO : List -> Queue & Stack
        if (walker.PathNodes.Count <= 0)
            return MethodResult.Success;

        var node = walker.PathNodes[0];
        var x = node.WorldPosition.x;
        var y = node.WorldPosition.y;

        Vector3 targetPos = new Vector3(x, y);
        var movePos = Time.deltaTime * speed;
        var nextPos = Vector3.MoveTowards(walker.transform.position, targetPos, movePos);

        if ((node.WorldPosition - (Vector2)nextPos).sqrMagnitude < 0.01f)
        {
            var removeNode = node;
            walker.transform.position = new Vector3(removeNode.WorldPosition.x, removeNode.WorldPosition.y);
            walker.PathNodes.RemoveAt(0);
            if (removeNode.CanInteraction == true)
            {
                if (removeNode.Interaction.Data.Universal.IsProactive == true)
                    walker.Interacte(removeNode.Interaction);
                else if (walker.PathNodes.Count == 0)
                    walker.Interacte(removeNode.Interaction);
            }
            else if (walker.CurrentEvent != null)
            {
                walker.CancelInteraction();
            }
        }
        else
        {
            walker.transform.position = nextPos;
        }

        return MethodResult.Running;
    }

    public void ChangePathNodeNonAlloc(Map mapHandler, Vector2Int start, Vector2Int goal, List<GroundNode> path)
    {
        var pivot = Vector2Int.RoundToInt(mapHandler.transform.position);
        start = path.Count == 0 ? start : path[0].WorldPosition;

        start -= pivot;
        goal -= pivot;

        if (goal.x >= 0 || goal.y >= 0 || goal.x < mapHandler.Size.x || goal.y < mapHandler.Size.y)
        {
            FindPath(mapHandler, start, goal);
            var goalNode = _current.Position == goal ? _current : _bestNode;
            ConnectMovementPath(goalNode, path);
            ResetPathNodeData();
        }
    }

    public void ChangePathNodeNonAlloc(Map mapHandler, Vector2 start, Vector2 goal, List<GroundNode> path)
    {
        Vector2Int intStart = Vector2Int.FloorToInt(start);
        Vector2Int intGoal = Vector2Int.FloorToInt(goal);
        ChangePathNodeNonAlloc(mapHandler, intStart, intGoal, path);
    }

    public bool IsPossiblePath(Map mapHandler, Vector2Int start, Vector2Int goal)
    {
        var pivot = Vector2Int.RoundToInt(mapHandler.transform.position);
        start -= pivot;
        goal -= pivot;

        if (goal.x >= 0 || goal.y >= 0 || goal.x < mapHandler.Size.x || goal.y < mapHandler.Size.y)
        {
            FindPath(mapHandler, start, goal);
            ResetPathNodeData();

            if (_current.Position == goal)
                return true;
        }
        return false;
    }

    private void FindPath(Map nodes, Vector2Int start, Vector2Int goal)
    {
        _minH = int.MaxValue;
        _openList.Enqueue(nodes.Nodes[start.x, start.y]);

        while (_openList.Count > 0 && _closeList.Count < 256)
        {
            _current = _openList.Dequeue();
            _closeList.Add(_current);
            if (_current.Position != goal)
                AddAroundPathNode(nodes, goal, _current);
            else // _current.Position == goal
                break;
        }
    }


    private void AddAroundPathNode(Map nodes, Vector2Int goal, GroundNode target)
    {
        int centerX = target.Position.x;
        int centerY = target.Position.y;

        int leftX = centerX - 1;
        int rightX = centerX + 1;
        int bottomY = centerY - 1;
        int topY = centerY + 1;

        TryAddPathNode(nodes, goal, leftX, centerY, MoveStarightCost);        // 4
        TryAddPathNode(nodes, goal, rightX, centerY, MoveStarightCost);       // 6
        TryAddPathNode(nodes, goal, centerX, bottomY, MoveStarightCost);      // 2
        TryAddPathNode(nodes, goal, centerX, topY, MoveStarightCost);         // 8

        TryAddPathNode(nodes, goal, leftX, bottomY, MoveDiagonalCost);        // 1
        TryAddPathNode(nodes, goal, leftX, topY, MoveDiagonalCost);           // 7 
        TryAddPathNode(nodes, goal, rightX, bottomY, MoveDiagonalCost);       // 3
        TryAddPathNode(nodes, goal, rightX, topY, MoveDiagonalCost);          // 9
    }

    private void TryAddPathNode(Map mapHandler, Vector2Int goal, int x, int y, int moveCost)
    {
        if (x < 0 || y < 0 || x >= mapHandler.Size.x || y >= mapHandler.Size.y) return;

        var node = mapHandler.Nodes[x, y];
        if (_closeList.Contains(node) || node.IsBlocked) return;
        if (moveCost == MoveDiagonalCost)
        {
            var currentPos = _current.Position;
            var conorPos = node.Position - currentPos;

            if (mapHandler.Nodes[currentPos.x + conorPos.x, currentPos.y].IsBlocked
             || mapHandler.Nodes[currentPos.x, currentPos.y + conorPos.y].IsBlocked)
            {
                return;
            }
        }

        if (_openList.Contains(node) == false)
            AddNewNodeToOpenList(node, goal, moveCost);
        else if (moveCost + _current.PathfindingData.G + node.PathfindingData.H < node.PathfindingData.F)
            RefreshNode(node, moveCost);
    }

    private void AddNewNodeToOpenList(GroundNode node, Vector2Int goal, int moveCost)
    {
        node.PathfindingData.H = CalculateHeuristic(node.Position, goal);
        RefreshNode(node, moveCost);
        _openList.Enqueue(node);
        if (node.PathfindingData.H < _minH)
        {
            _bestNode = node;
            _minH = node.PathfindingData.H;
        }
    }

    private void RefreshNode(GroundNode node, int moveCost)
    {
        node.PathfindingData.BeforeNode = _current;
        var data = node.PathfindingData;
        int g = moveCost + _current.PathfindingData.G;
        int h = data.H;
        int f = data.G + h;
        node.PathfindingData = new(_current, data.QueueIndex, g, h, f);
    }

    private void ConnectMovementPath(GroundNode node, List<GroundNode> path)
    {
        path.Clear();
        path.Add(node);
        var count = 0;
        while (node.PathfindingData.BeforeNode != null && count < 1000)
        {
            count++;
            node = node.PathfindingData.BeforeNode;
            path.Add(node);
        }
        path.Reverse();
    }

    private void ResetPathNodeData()
    {
        foreach (var node in _closeList)
        {
            node.ResetPriorityData();
        }
        _closeList.Clear();
        _openList.Clear();
    }

    private int CalculateHeuristic(Vector2Int start, Vector2Int end)
    {
        int disX = Mathf.Abs(start.x - end.x);
        int disY = Mathf.Abs(start.y - end.y);
        return (disX + disY) << Weight;
    }
}
