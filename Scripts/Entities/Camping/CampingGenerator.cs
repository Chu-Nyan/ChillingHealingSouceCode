using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CampingGenerator : Singleton<CampingGenerator>
{
    private Dictionary<CampingType, CampingUniversalData> _datas;
    private ObjectPooling<Camping> _pooling;
    private Camping _new;

    private Map _targetMap;

    public CampingGenerator()
    {
        var json = AssetManager.Instance.MergeJsonsSync(Const.Json_CampingDB, Const.Json_ActiveCampingDB);
        var converter = new CampingTypeConverter<CampingUniversalData, ActiveCampingData>("Type", 0, CampingType.Eat);
        _datas = AssetManager.Instance.DeserializeJsonSync<Dictionary<CampingType, CampingUniversalData>>(json, converter);
        DeserializeLevelData();
        _pooling = new(
            () =>
            {
                return AssetManager.Instance.GenerateLoadAssetSync<Camping>(Const.CampingPrefab, "CampingPrefab");
            }, (i) => { i.gameObject.SetActive(true); });
    }

    public CampingGenerator Generate(CampingType type, Map targetMap, int level)
    {
        _new = _pooling.Dequeue();
        _new.Destroyed += Destroy;
        _new.SetData(new() { Type = type });
        _new.Data.Universal = _datas[type];
        _new.Data.Level = level;
        _targetMap = targetMap;
        SetStrategy();
        return this;
    }

    public CampingGenerator Generate(CampingData data, Map targetMap)
    {
        _new = _pooling.Dequeue();
        _new.Destroyed += Destroy;
        _new.SetData(data);
        _targetMap = targetMap;
        SetStrategy();
        return this;
    }

    public CampingGenerator SetSprite(bool isLock, bool isBlocked, Sprite sprite = null)
    {
        if (sprite == null || isLock == false)
            sprite = GetSprite(_new.Data.Universal.Type);

        _new.SetSprite(sprite);
        return this;
    }

    public CampingGenerator SetPosition(Vector3 pos, Rect interacteZone)
    {
        _new.SetPosition(pos);
        _new.Data.InteractZone = interacteZone;
        return this;
    }

    public Camping GetNewCamping()
    {
        _new.Unlock(_new.Data.Level);
        return _new;
    }

    public bool TryGetActiveData(CampingType type, out ActiveCampingData data)
    {
        var result = _datas.TryGetValue(type, out var camping) && camping is ActiveCampingData;
        data = camping as ActiveCampingData;
        return result;

    }

    public CampingUniversalData GetData(CampingType type)
    {
        return _datas[type];
    }

    private CampingGenerator SetStrategy()
    {
        var strategy = SetStrategy(_targetMap);

        _new.Init(strategy);
        return this;
    }

    private ICampingStrategy SetStrategy(Map map)
    {
        ICampingStrategy strategy = null;
        var type = _new.Data.Type;

        if (type <= CampingType.Eat)
            strategy = SetCampingStrategy(_new.Data);
        else if (type <= CampingType.ATM)
            strategy = SetUIOpenStrategy(type);
        else if (type < CampingType.Teleport)
            strategy = SetPickUpItemStrategy(type);
        else if (type == CampingType.Teleport)
            strategy = SetTeleportStrategy(map);

        return strategy;
    }

    private ICampingStrategy SetCampingStrategy(CampingData data)
    {
        var strategy = new ActiveCampingStrategy();
        strategy.Init(data);
        return strategy;
    }

    private ICampingStrategy SetUIOpenStrategy(CampingType type)
    {
        var strategy = new UIOpenStrategy();
        strategy.Init(_new, type);
        return strategy;
    }

    private ICampingStrategy SetPickUpItemStrategy(CampingType type)
    {
        var strategy = new PickUpItemStrategy();
        int random = UnityEngine.Random.Range(0, 10);
        var itemType = random == 0 ? ItemType.FourLeafClover : ItemType.ThreeLeafClover;
        strategy.Init(itemType, 1);
        return strategy;
    }

    private ICampingStrategy SetTeleportStrategy(Map map)
    {
        MapCategory targetMap = map.Type < MapType.Car ? MapCategory.Inside : MapCategory.Outside;
        var strategy = new TeleportStrategy();
        strategy.Init(targetMap);
        return strategy;
    }

    public Sprite GetSprite(CampingType type)
    {
        var spriteAtlas = AssetManager.Instance.LoadAssetSync<SpriteAtlas>(Const.Sptire_Camping);
        return spriteAtlas.GetSprite($"{type}");
    }

    private void Destroy(Camping camping)
    {
        _pooling.Enqueue(camping);
    }

    private void DeserializeLevelData()
    {
        var helper = AssetManager.Instance.DeserializeJsonSync<List<LevelDataHelper>>(Const.Json_CampingLevelDB);
        for (int i = 0; i < helper.Count; i++)
        {
            var type = (CampingType)helper[i].Type;

            if (_datas[type] is ActiveCampingData active)
            {
                if (active.LevelData == null)
                {
                    active.LevelData = new();
                }

                active.LevelData.Add(new(helper[i]));
            }
        }
    }

    public CampingData GenerateCampingData(CampingType type, int level)
    {
        var data = _datas[type];
        return new() { Type = type, Level = level, Universal = data };
    }
}
