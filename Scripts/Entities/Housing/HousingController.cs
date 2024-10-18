using UnityEngine;

public class HousingController
{
    private Player _player;
    private ClickModeChanger _clickHandler;
    private Blueprint _blueprint;

    private IBuildable _selectObject;
    private HousingUI _housingUI;

    public Blueprint Blueprint
    {
        get => _blueprint;
    }

    public HousingController()
    {
        _blueprint = AssetManager.Instance.GenerateLoadAssetSync<Blueprint>(Const.Blueprint);
        _blueprint.Destroy();
    }

    public void Init(ClickModeChanger click, Player player)
    {
        _player = player;
        _clickHandler = click;

        _housingUI = UIManager.Instance.GetUI<HousingUI>(UIName.HousingUI);
        _housingUI.Init(player.Inventory.Space.ItemList[ItemCategory.Furniture], ShowBlueprint, StopHousingMode, GenerateBlueprint);
        _housingUI.InitSelectObjectBtn(Rotate, Remove);
        _housingUI.Deactivate();
    }

    public void StartHousingMode()
    {
        _player.SwtichSprite(false);
        UIManager.Instance.ChangeHUDUIMode(HUDChanger.Mode.Housing);
        var mode = _clickHandler.ChangeMode<HousingClickHandler>(ClickMode.Housing);
        mode.PickCanceled += RecordHistory;
        mode.Pick += Select;
    }

    public void StopHousingMode()
    {
        _player.SwtichSprite(true);
        _clickHandler.ChangeMode(ClickMode.Player);
        _blueprint.Destroy();
        UIManager.Instance.ChangeHUDUIMode(HUDChanger.Mode.Inside);
    }

    private void Select(IBuildable obj)
    {
        _selectObject = obj;
        _housingUI.SwitchRotateButton(obj.CanRotation);
    }

    private void Rotate(int dir)
    {
        if (_selectObject == null)
            return;

        if (_selectObject.IsBlocked == true)
        {
            BlockedObject(dir);
        }
        else
        {
            GroundObjectRotate(dir);
        }
    }

    private void GroundObjectRotate(int dir)
    {
        var size = new Vector2(_selectObject.Size[1], _selectObject.Size[0]);
        if (_player.CurrentMap.IsGroundObjectNode(size, _selectObject.Position) == true)
        {
            if (_selectObject is FurnitureObject)
            {
                _player.CurrentMap.RemoveBlocked(_selectObject);
                _selectObject.Rotate(dir);
                _player.CurrentMap.ChangeBlockedWithObject(_selectObject);
            }
            else
            {
                _selectObject.Rotate(dir);
            }
        }
    }

    private void BlockedObject(int dir)
    {
        var size = new Vector2(_selectObject.Size[1], _selectObject.Size[0]);
        if (_player.CurrentMap.IsMovableNode(size, _selectObject.Position, _selectObject.Size) == true)
        {
            if (_selectObject is FurnitureObject)
            {
                _player.CurrentMap.RemoveBlocked(_selectObject);
                _selectObject.Rotate(dir);
                _player.CurrentMap.ChangeBlockedWithObject(_selectObject);
            }
            else
            {
                _selectObject.Rotate(dir);
            }
        }
    }

    private void RecordHistory(IBuildable clickable, Vector3 beforeLocalPos)
    {
        SoundManager.Instance.PlaySE(SoundType.InteriorPlacement);
        if (clickable is FurnitureObject obj)
        {
            var map = _player.CurrentMap;
            map.RemoveBlocked(clickable.Size, beforeLocalPos, clickable.IsBlocked);
            map.ChangeBlockedWithObject(obj);
            if (map.CheckValidation(_blueprint.Size, _blueprint.Position) == false)
            {
                _blueprint.SetCanBuildWithSpriteColor(false);
            }
        }
    }

    private void ShowBlueprint(Furniture data)
    {
        _blueprint.Show(data);
        _blueprint.Move(_player.transform.position, _player.transform.position - _player.CurrentMap.transform.position);
    }

    public void GenerateBlueprint()
    {
        if (_blueprint.gameObject.activeSelf == false || _blueprint.CanBuild == false)
        {
            // TODO 오류 로그
        }
        else
        {
            var furniture = AssetManager.Instance.GenerateLoadAssetSync<FurnitureObject>(Const.FurnitureGameObj);
            furniture.Init(_blueprint);
            _blueprint.Build();
            _player.CurrentMap.AddFurnitureObject(furniture);

        }
    }

    private void Remove()
    {
        if (_selectObject == null)
            return;

        if (_selectObject is FurnitureObject furniture)
        {
            var data = furniture.Data;
            _player.AddItem(data.Universal.Type, 1);
            _player.CurrentMap.RemoveBlocked(_selectObject);
        }
        _selectObject?.Destroy();
        _selectObject = null;
        _housingUI.SwitchRotateButton(false);

        SoundManager.Instance.PlaySE(SoundType.InteriorTrashcan);
    }
}
