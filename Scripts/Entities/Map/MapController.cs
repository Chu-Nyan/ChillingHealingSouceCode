using System;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private Map _outside;
    [SerializeField] private Map _inside;
    [SerializeField] private PolygonCollider2D _polygon;

    private Dictionary<MapType, MapData> _mapDatas;
    private GroundNode[,] _outsideNodes;
    private GroundNode[,] _insideNodes;
    private Map _currentMap;
    private CloverRespawner _cloverRespawner;

    public event Action<Map> MapChanged;

    public Dictionary<MapType, MapData> MapDatas
    {
        get => _mapDatas;
    }

    public Map OutsideMap
    {
        get => _outside;
    }

    public Map InsideMap
    {
        get => _inside;
    }

    public Map CurrentMap
    {
        get => _currentMap;
    }

    public PolygonCollider2D PolygonCollider
    {
        get => _polygon;
    }

    private void Awake()
    {
        _outsideNodes = GenerateNodeMap(100, _outside.transform.position);
        _insideNodes = GenerateNodeMap(50, _inside.transform.position);
        _cloverRespawner = new();
        _cloverRespawner.SetTargetMap(_outside);
        _mapDatas = LoadMapData();
        SaveManager.Instance._mapController = this;

        if (SaveManager.Instance.IsSaveFileLoaded)
            new OfflineReward(_mapDatas);
    }

    public void InitBeginningMap(Player player)
    {
        _outside.Init(_outsideNodes);
        _inside.Init(_insideNodes);
        LoadMap(player.Data.OutsideMap);

        for (int i = 0; i < player.Data.CampingCarLevel; i++)
        {
            LoadCampingCarLevelMap(i + 1);
        }
        _inside.RefreshFurniture();
        _currentMap = _outside;
    }

    public void ChangeMap(MapType type, Player player)
    {
        MapCategory category = type < MapType.Car ? MapCategory.Outside : MapCategory.Inside;
        _currentMap = GetCategoryMap(category);
        if (_currentMap.Type != type && category == MapCategory.Outside)
        {
            LoadMap(type);
        }
        _currentMap.Enter(player);
        var mode = category == MapCategory.Outside ? HUDChanger.Mode.Outside : HUDChanger.Mode.Inside;
        UIManager.Instance.ChangeHUDUIMode(mode);
        OnMapChanged(_currentMap);
    }

    public void ChangeMap(MapCategory category, Player player)
    {
        _currentMap = GetCategoryMap(category);
        _currentMap.Enter(player);
        var mode = category == MapCategory.Outside ? HUDChanger.Mode.Outside : HUDChanger.Mode.Inside;
        UIManager.Instance.ChangeHUDUIMode(mode);
        OnMapChanged(_currentMap);
    }

    public void UpgradeCampingCar(int level)
    {
        LoadCampingCarLevelMap(level);
        OnMapChanged(_currentMap);
    }

    public Map GetCategoryMap(MapCategory category)
    {
        return category == MapCategory.Inside ? _inside : _outside;
    }

    private void LoadMap(MapType type)
    {
        if (type == MapType.Forest)
        {
            _outside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_Forest).GetComponent<TilemapHandler>(), _mapDatas[type]);
        }
        else if (type == MapType.Sea)
        {
            _outside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_Sea).GetComponent<TilemapHandler>(), _mapDatas[type]);
        }
        else if (type >= MapType.Car)
        {
            _inside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_CarLv1).GetComponent<TilemapHandler>(), _mapDatas[type]);
        }
    }

    private void LoadCampingCarLevelMap(int level)
    {
        if (level == 1)
        {
            _inside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_CarLv1).GetComponent<TilemapHandler>(), _mapDatas[MapType.Car], false);
        }
        else if (level == 2)
        {
            _inside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_CarLv2).GetComponent<TilemapHandler>(), _mapDatas[MapType.Car], true);
        }
        else if (level == 3)
        {
            _inside.InitHandler(AssetManager.Instance.LoadAssetSync<GameObject>(Const.Map_CarLv3).GetComponent<TilemapHandler>(), _mapDatas[MapType.Car], true);
        }
    }

    private GroundNode[,] GenerateNodeMap(int nodeSize, Vector3 worldPos)
    {
        var nodes = new GroundNode[nodeSize, nodeSize];
        var pivot = new Vector2Int((int)worldPos.x, (int)worldPos.y);
        for (int i = 0; i < nodeSize; i++)
        {
            for (int j = 0; j < nodeSize; j++)
            {
                var pos = new Vector2Int(i, j);
                nodes[i, j] = new GroundNode(pos, pivot + pos);
            }
        }

        return nodes;
    }

    private Dictionary<MapType, MapData> LoadMapData()
    {
        Dictionary<MapType, MapData> mapData = null;
        if (mapData == null)
        {
            bool isSaveFileLoaded = SaveManager.Instance.IsSaveFileLoaded;
            Dictionary<MapType, MapSaveData> saveData =
                isSaveFileLoaded ? SaveManager.Instance._saveData.MapDictionary : null;
            mapData = Utilities.NewEnumKeyDictionary<MapType, MapData>();
            foreach (var item in mapData)
            {
                item.Value.Type = item.Key;
            }

            var originData = AssetManager.Instance.DeserializeJsonSync<List<OriginMapDataLoadHelper>>(Const.Json_MapDB);
            foreach (var item in originData)
            {
                var value = mapData[item.MapType];
                value.IsUnlock = isSaveFileLoaded == true ? saveData[item.MapType].IsUnlocked : false;
                var campingData = new CampingData
                {
                    Type = item.CampingType,
                    Level = isSaveFileLoaded ? saveData[item.MapType].CampingLevelList[item.CampingType] : item.Level
                };
                campingData.Universal = CampingGenerator.Instance.GetData(item.CampingType);
                value.Campings.Add(campingData);
                if (CampingGenerator.Instance.TryGetActiveData(item.CampingType, out var activedata))
                {
                    if (NPCGenerator.Instance.TryGetNpcData(activedata.NPC, out var npcData))
                    {
                        if (isSaveFileLoaded)
                            npcData.Level = saveData[item.MapType].NPCLevelList[npcData.Type];
                        value.Npcs.Add(npcData);
                    }
                }
            }

            if (saveData != null)
            {
                foreach (var item in saveData)
                {
                    var type = item.Key;
                    var mapDatas = item.Value;
                    foreach (var item1 in mapDatas.FurnitureList)
                    {
                        if (ItemGenerator.Instance.TryGetFurnitureData(item1.Type, out var universalData) == true)
                        {
                            var data = new FurnitureData();
                            data.Load(item1, universalData);
                            mapData[type].Furnitures.Add(data);
                        }
                    }
                }
            }
        }

        return mapData;
    }

    private void OnMapChanged(Map map)
    {
        if (map.TryPlayMapThemeMusic() == false)
        {
            _outside.TryPlayMapThemeMusic();
        }
        SetCameraRange(map);
        MapChanged?.Invoke(map);
    }

    private void SetCameraRange(Map map)
    {
        _polygon.points = map.CemeraZone;
        _polygon.offset = map.transform.position;
    }
}

public class OriginMapDataLoadHelper
{
    public MapType MapType;
    public CampingType CampingType;
    public int Level;
}