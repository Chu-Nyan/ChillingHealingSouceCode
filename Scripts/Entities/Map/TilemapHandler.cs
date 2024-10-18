using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapHandler : MonoBehaviour
{
    [SerializeField] private Transform CampingRoot;
    public PolygonCollider2D PolygonCollider;

    public Tilemap Ground;
    public Tilemap Wall;
    public Tilemap Front;
    public Tilemap Behind;
    public List<CampingHandler> CampingHandler;

    public MapType Type;

    public List<CampingHandler> LoadCampingHandler()
    {
        return CampingRoot.GetComponentsInChildren<CampingHandler>().ToList();
    }
}
