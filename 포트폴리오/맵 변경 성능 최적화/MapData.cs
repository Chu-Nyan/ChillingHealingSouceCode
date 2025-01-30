using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public static readonly Dictionary<MapType, SoundType> SoundTheme = new()
    {
        { MapType.Forest, SoundType.Potato },
        { MapType.Sea, SoundType.SeasideVillageAmbience },
    };

    public const int MaxResources = 100;
    public MapType Type;

    public List<CampingData> Campings;
    public List<NPCData> Npcs;
    public List<FurnitureData> Furnitures;
    public bool _isUnlock;
    private float _resources;

    public bool IsUnlock
    {
        get => _isUnlock;
        set => _isUnlock = value;
    }

    public float SetResources
    {
        get => _resources;
        set => _resources = Mathf.Clamp(value, 0, MaxResources);
    }

    public MapData()
    {
        Campings = new();
        Npcs = new();
        Furnitures = new();
    }
}