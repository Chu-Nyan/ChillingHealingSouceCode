using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviourSingleton<NPCGenerator>
{
    private MapController _controller;
    private ObjectPooling<NPC> _pooling;
    private Dictionary<UnitType, NPCData> _datas;
    private Dictionary<UnitType, DialogSystem> _dialogSys;
    public event Action<NPC> Generated;

    private NPC _new;

    protected override void Awake()
    {
        base.Awake();
        _pooling = new(() => 
        { 
            return AssetManager.Instance.GenerateLoadAssetSync<NPC>(Const.NPCPrefab); 
        }, (i) =>
        {
            i.gameObject.SetActive(true);
        }

        );
        _datas = AssetManager.Instance.DeserializeJsonSync<Dictionary<UnitType, NPCData>>(Const.Json_NPCDB);
        var items = AssetManager.Instance.DeserializeJsonSync<List<LevelData>>(Const.Json_NPCLevelDB);
        _dialogSys = LoadDialog();
        NPCData.LevelData = items;
    }

    public void Init(MapController mapController)
    {
        _controller = mapController;
    }

    public NPCGenerator Ready(UnitType type)
    {
        _new = _pooling.Dequeue();
        _new.OnDestroyed += EnqueuePooling;
        SetStatus(type);
        return this;
    }

    public NPCGenerator SetPosition(Vector3 pos)
    {
        _new.transform.position = pos;
        Map map = pos.x < 200 ? _controller.OutsideMap : _controller.InsideMap;
        _new.InitMap(map);
        return this;
    }

    public NPCGenerator RandomPositionInRange(Vector3 pivot, int range)
    {
        Map map = pivot.x < 200 ? _controller.OutsideMap : _controller.InsideMap;
        for (int i = 0; i < 100; i++)
        {
            var pos = Utilities.RandomVector3InRange(pivot, range);
            if (map.IsMoveableNode(pos) == true)
            {
                SetPosition(pos);
            }
        }

        return this;
    }

    public NPCGenerator SetPatrolPosition(Vector3 pos)
    {
        _new.PatrolPosition = pos;
        return this;
    }

    public NPC Generate()
    {
        Generated?.Invoke(_new);
        return _new;
    }

    private void SetStatus(UnitType type)
    {
        _new.InitStatus(_datas[type], _dialogSys[type]);
    }

    public Sprite GetSprite(UnitType type)
    {
        string partNum = $"{type.ToString()}";
        Sprite single = AssetManager.Instance.LoadAssetSync<Sprite>($"{partNum}[{partNum}_{0}]");
        return single;
    }

    public bool TryGetNpcData(UnitType type, out NPCData data)
    {
        return _datas.TryGetValue(type, out data);
    }

    public Dictionary<UnitType, DialogSystem> LoadDialog()
    {
        var dic = new Dictionary<UnitType, DialogSystem>();
        var dialogs = AssetManager.Instance.DeserializeJsonSync<List<DialogLoadHelper>>(Const.Json_NPCDialogDB);
        foreach (var item in dialogs)
        {
            if (dic.TryGetValue((UnitType)item.Owner, out var system) == false)
            {
                system = new();
                dic.Add((UnitType)item.Owner, system);
            }
            system.AddText(item.DialogType, item.Text, item.Case, item.Order);
        }

        return dic;
    }

    private void EnqueuePooling(NPC npc)
    {
        _pooling.Enqueue(npc);
    }
}
