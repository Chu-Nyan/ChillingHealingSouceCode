using UnityEngine;

public class WorldMapMenu : MenuBase
{
    private static readonly string _title = "월드맵";
    private static readonly Vector2 _gridCellSize = new(900, 250);
    private static readonly TextAnchor _anchor = TextAnchor.UpperCenter;
    private MapController _controller;
    private Player _player;

    public override void Init()
    {
        Title = _title;
        Tabs = new string[] { };
        GridCellSize = _gridCellSize;
        Anchor = _anchor;
        _controller = UIManager.Instance._mapController;
        _player = UIManager.Instance._player;
    }

    public override void Refresh(GameObject slotParent, int currentTab)
    {
        // 하드코딩 ^,^
        for (int i = 0; i < 2; i++)
        {
            var slot = _UIM.GetPooledUI<WorldMapSlotUI>(PooledUIName.WorldMapSlotUI);
            MapType type;
            if (i == 0)
                type = MapType.Forest;
            else if (i == 1)
                type = MapType.Sea;
            else
                type = MapType.Car;

            slot.Init(type, (i) =>
            {
                var menu = UIManager.Instance.GetUI<MenuUI>(UIName.MenuUI);
                menu.Deactivate();
                _player.MoveAnotherOutSideMap(type);

            });
            slot.transform.SetParent(slotParent.transform, false);
            slot.transform.SetAsLastSibling();
            Slots.Add(slot);
        }

    }
}
