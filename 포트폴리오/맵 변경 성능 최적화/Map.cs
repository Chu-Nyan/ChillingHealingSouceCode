using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public const int MaxResources = 100;

    [SerializeField] private Tilemap _groundMap;
    [SerializeField] private Tilemap _frontMap;
    [SerializeField] private Tilemap _blockMap;
    [SerializeField] private Tilemap _behindMap;
    
    private MapType _type;
    private MapData _data;
    private GroundNode[,] _nodes;
    private List<Camping> _campings;
    private List<NPC> _npcs;
    private List<FurnitureObject> _furnitures;
    private Camping _teleport;
    private Vector2Int _size;
    private Vector2[] _cameraZone;
    
    private float _resources;

    public MapType Type
    {
        get => _type;
    }

    public GroundNode[,] Nodes
    {
        get => _nodes;
    }

    public List<NPC> NPCs
    {
        get => _npcs;
    }

    public List<FurnitureObject> Furnitures
    {
        get => _furnitures;
    }

    public Vector2Int Size
    {
        get => _size;
        set => _size = value;
    }

    public float Resources
    {
        get => _resources;
        set => _resources = Mathf.Min(value, MaxResources);
    }

    public Vector2[] CemeraZone
    {
        get => _cameraZone;
    }

    public Camping Teleport
    {
        get => _teleport;
    }

    private void Awake()
    {
        _furnitures = new();
        _campings = new();
        _npcs = new();
        NPCGenerator.Instance.Generated += (i) =>
        {
            if (i.Map == this)
            {
                _npcs.Add(i);
                i.OnDestroyed += RemoveNPC;
            }
        };
    }

    private void RemoveNPC(NPC npc)
    {
        _npcs.Remove(npc);
    }

    public void Init(GroundNode[,] nodes)
    {
        _nodes = nodes;
    }

    public bool TryPlayMapThemeMusic()
    {
        var hasMusic = MapData.SoundTheme.TryGetValue(_data.Type, out var value);
        if (hasMusic == true)
        {
            SoundManager.Instance.PlayBGM(value);
        }

        return hasMusic;
    }

    public void InitHandler(TilemapHandler handler, MapData mapData, bool IsOverwrite = false)
    {
        _type = handler.Type;
        _data = mapData;
        _data.IsUnlock = true;
        _cameraZone = handler.PolygonCollider.points;
        if (IsOverwrite == false)
            ClearAllObject();
        RefreshMap(handler.Ground, handler.Wall, handler.Front, handler.Behind, IsOverwrite);
        var campingHandlers = handler.LoadCampingHandler();
        GenerateCampinghandler(campingHandlers, mapData.Campings);
    }

    public Camping AddCamping(Camping camping)
    {
        camping.transform.SetParent(transform, false);
        camping.Destroyed += RemoveBlocked;
        camping.Destroyed += RemoveInteraction;
        ChangeBlockedWithObject(camping);
        ChangeInteractionZoneWithObject(camping);

        return camping;
    }

    private Camping GenerateCamping(CampingHandler handler, CampingData campingData)
    {
        var isLock = campingData.Level == 0;
        var camping = CampingGenerator.Instance.Generate(campingData, this)
            .SetSprite(isLock, campingData.Universal.IsBlocked, handler.SpriteRenderer.sprite)
            .SetPosition(handler.transform.position, handler.InteractRect)
            .GetNewCamping();

        return AddCamping(camping);
    }

    public void Enter(Player player)
    {
        player.transform.position = Vector3Int.FloorToInt(_teleport.transform.position);
    }

    public void RefreshMap(Tilemap newGround, Tilemap newWall, Tilemap frontMap, Tilemap behind, bool isOverwrite)
    {
        RefreshRayer(_groundMap, newGround, false, isOverwrite);
        RefreshRayer(_frontMap, frontMap, false, isOverwrite);
        RefreshRayer(_behindMap, behind, false, isOverwrite);
        RefreshRayer(_blockMap, newWall, true, isOverwrite);

        var sizeBlock = _blockMap.size;
        var groundMap = _blockMap.size;

        if (isOverwrite == true)
        {
            OverwriteRemoveProcess(_groundMap, _blockMap);
        }
        _size = new Vector2Int(math.max(sizeBlock.x, groundMap.x), math.max(sizeBlock.y, groundMap.y));
    }

    public void RefreshFurniture()
    {
        LoadFurniture(_data.Furnitures);
    }

    private void RefreshRayer(Tilemap viewer, Tilemap target, bool isBlocked, bool isOverwrite)
    {
        if (isOverwrite == false)
            viewer.ClearAllTiles();

        target.CompressBounds();
        var startPos = (Vector2Int)target.cellBounds.position;
        var size = (Vector2Int)target.cellBounds.size + startPos;

        for (int y = startPos.y; y <= size.y; y++)
        {
            for (int x = startPos.x; x <= size.x; x++)
            {
                var vetor3int = new Vector3Int(x, y, 0);
                TileBase copy;
                if (target.HasTile(vetor3int) == false)
                    continue;

                copy = target.GetTile(vetor3int);
                _nodes[x, y].IsBlocked = isBlocked;
                viewer.SetTile(vetor3int, copy);
            }
        }
    }

    private void OverwriteRemoveProcess(Tilemap currentGround, Tilemap afterWall)
    {
        var startPos = (Vector2Int)afterWall.cellBounds.position;
        var size = (Vector2Int)afterWall.cellBounds.size + startPos;

        for (int y = startPos.y; y <= size.y; y++)
        {
            for (int x = startPos.x; x <= size.x; x++)
            {
                var vetor3int = new Vector3Int(x, y, 0);
                if (currentGround.HasTile(vetor3int) == false)
                    continue;

                afterWall.SetTile(vetor3int, null);
            }
        }
    }

    public void AddFurnitureObject(FurnitureObject obj, bool isLoad = false)
    {
        ChangeBlockedWithObject(obj.Size, obj.Data.Position, obj.IsBlocked);
        if (_furnitures.Contains(obj) == false)
        {
            obj.transform.SetParent(transform, false);
            obj.Destroyed += RemoveFurniture;

            _furnitures.Add(obj);
            if (isLoad == false)
            {
                _data.Furnitures.Add(obj.Data);
            }
        }
    }

    public bool IsMoveableNode(Vector3 worldPos)
    {
        int x = (int)worldPos.x;
        int y = (int)worldPos.y;

        if (Size.x < x || Size.y < y || x < 0 || y < 0)
            return false;

        return _nodes[x, y].IsBlocked == false;
    }

    public bool IsInteracteNode(Vector3 worldPos)
    {
        int x = (int)worldPos.x;
        int y = (int)worldPos.y;

        if (Size.x < x || Size.y < y || x < 0 || y < 0)
            return false;

        return _nodes[x, y].CanInteraction;
    }

    public void ChangeBlockedWithObject(int[] size, Vector3 pos, bool isBlocked)
    {
        int startX = (int)pos.x;
        int startY = (int)pos.y;
        int endX = startX + size[0];
        int endY = startY + size[1];

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                if (isBlocked == false)
                {
                    _nodes[x, y].WasGroundObject = !isBlocked;
                }
                else
                {
                    _nodes[x, y].IsBlocked = isBlocked;
                }
            }
        }
    }

    public void ChangeBlockedWithObject(IBuildable able)
    {
        ChangeBlockedWithObject(able.Size, able.Position, able.IsBlocked);
    }

    private void ChangeBlockedWithObject(Camping obj)
    {
        ChangeBlockedWithObject(obj.Data.Universal.Size, obj.transform.localPosition, obj.Data.Universal.IsBlocked);
    }

    private void ChangeInteractionZoneWithObject(Camping obj)
    {
        int startX = (int)(obj.Data.InteractZone.xMin);
        int startY = (int)(obj.Data.InteractZone.yMin);
        int endX = (int)(obj.Data.InteractZone.xMax);
        int endY = (int)(obj.Data.InteractZone.yMax);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                _nodes[x, y].RegisterInteraction(obj);
            }
        }
    }

    public void RemoveBlocked(int[] size, Vector3 pos, bool isBlocked)
    {
        int startX = (int)pos.x;
        int startY = (int)pos.y;
        int endX = startX + size[0];
        int endY = startY + size[1];

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                if (isBlocked == false)
                {
                    _nodes[x, y].WasGroundObject = false;
                }
                else
                {
                    _nodes[x, y].IsBlocked = false;
                }
            }
        }
    }

    public void RemoveBlocked(IBuildable able)
    {
        RemoveBlocked(able.Size, able.Position, able.IsBlocked);
    }

    private void RemoveBlocked(Camping obj)
    {
        RemoveBlocked(obj.Data.Universal.Size, obj.transform.localPosition, obj.Data.Universal.IsBlocked);
    }

    private void RemoveInteraction(Camping obj)
    {
        int startXX = (int)(obj.Data.InteractZone.xMin - transform.position.x);
        int startYY = (int)(obj.Data.InteractZone.yMin - transform.position.y);
        int endXX = (int)(obj.Data.InteractZone.xMax - transform.position.x);
        int endYY = (int)(obj.Data.InteractZone.yMax - transform.position.y);

        for (int y = startYY; y < endYY; y++)
        {
            for (int x = startXX; x < endXX; x++)
            {
                _nodes[x, y].UnregisterInteraction(obj);
            }
        }
    }

    public bool IsMovableNode(Vector2 size, Vector3 pos)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (IsMoveableNode(new Vector3(pos.x + x, pos.y + y)) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsInteractionZone(Vector2 size, Vector3 pos)
    {
        int posX = (int)pos.x;
        int posY = (int)pos.y;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (_nodes[x + posX, y + posY].CanInteraction == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsMovableNode(Vector2 size, Vector3 pos, int[] beforeSize)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x < beforeSize[0] && y < beforeSize[1])
                    continue;

                if (IsMoveableNode(new Vector3(pos.x + x, pos.y + y)) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsGroundObjectNode(Vector2 size, Vector3 pos)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (_groundMap.HasTile(new Vector3Int((int)(pos.x + x), (int)(pos.y + y))) == false)
                    return false;

                if (_nodes[(int)(pos.x + x), (int)(pos.y + y)].WasGroundObject == true)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool CheckValidation(int[] size, Vector3 pos)
    {
        return IsMovableNode(new Vector2(size[0], size[1]), pos);
    }

    public bool CheckInteractionZone(int[] size, Vector3 pos)
    {
        return IsInteractionZone(new Vector2(size[0], size[1]), pos);
    }

    private void GenerateCampinghandler(List<CampingHandler> handler, List<CampingData> campingData)
    {
        foreach (var item in handler)
        {
            CampingData data = null;
            for (int i = 0; i < campingData.Count; i++)
            {
                if (campingData[i].Type == item.Type)
                {
                    data = campingData[i];
                    break;
                }
            }
            if (data == null)
            {
                Debug.Log(item.Type);
            }

            var camping = GenerateCamping(item, data);
            if (item.Type == CampingType.Teleport)
            {
                _teleport = camping;
            }

            _campings.Add(camping);
        }
    }

    private void LoadFurniture(List<FurnitureData> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            var obj = AssetManager.Instance.GenerateLoadAssetSync<FurnitureObject>(Const.FurnitureGameObj);
            obj.Init(data[i]);
            AddFurnitureObject(obj, true);
        }
    }

    private void RemoveFurniture(FurnitureObject obj)
    {
        _furnitures.Remove(obj);
        _data.Furnitures.Remove(obj.Data);
    }

    private void ClearAllObject()
    {
        for (int i = _campings.Count - 1; i >= 0; i--)
        {
            _campings[i].Destory();
        }
        for (int i = _npcs.Count - 1; i >= 0; i--)
        {
            _npcs[i].Destroy();
        }
        for (int i = _furnitures.Count - 1; i >= 0; i--)
        {
            _furnitures[i].Destroy();
        }
    }
}
