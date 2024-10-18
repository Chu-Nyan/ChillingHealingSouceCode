using System;
using System.Collections.Generic;
using UnityEngine;

public class HousingClickHandler : MonoBehaviour, IClickHandler
{
    private InputManager _im;
    private Player _player;
    private List<FurnitureObject> _furnitures;
    private Blueprint _blueprint;
    private IBuildable _pickObject;
    private Vector3Int _beforeCheckPos;
    private Vector3 _startPos;
    private bool canBuild;
    private Coroutine _screenDrag;

    public event Action<IBuildable> Pick;
    public event Action<IBuildable, Vector3> PickCanceled;
    public event Action<MissionType, int, int> BehaviorEvent;

    public ClickMode ClickMode
    {
        get => ClickMode.Housing;
    }

    public void Init(Player player, Blueprint blueprint)
    {
        _im = InputManager.Instance;
        _player = player;
        _blueprint = blueprint;
    }

    public void Register()
    {
        InputManager.Instance.RegisterTouch0Performed(ClickBlueprint);
        _furnitures = _player.CurrentMap.Furnitures;
    }

    public void Unregister()
    {
        _player.CameraArm.localPosition = Vector3.zero;
        InputManager.Instance.UnregisterTouch0Performed(ClickBlueprint);
        PickCanceled = null;
    }

    private void ClickBlueprint(Vector2 screen, bool isPointerOverUI)
    {
        if (isPointerOverUI == true)
            return;

        var world = (Vector2)Camera.main.ScreenToWorldPoint(screen);
        bool isSearch = false;
        if (_blueprint.IsTouchOnSprite(world) && _blueprint.gameObject.activeSelf == true)
        {
            _pickObject = _blueprint;
            isSearch = true;
        }
        else
        {
            IBuildable groundObj = null;
            IBuildable selectObj = null;
            foreach (var item in _furnitures)
            {
                if (item.IsTouchOnSprite(world))
                {
                    isSearch = true;
                    if (item.IsBlocked == true)
                    {
                        selectObj = item;
                        break;
                    }
                    else
                    {
                        groundObj = item;
                    }

                }
            }
            selectObj ??= groundObj;
            _pickObject = selectObj;
        }

        if (isSearch == true)
        {
            _startPos = _pickObject.transform.position;
            Pick?.Invoke(_pickObject);
            InputManager.Instance.RegisterDragPerformed(MoveObject);
            InputManager.Instance.RegisterTouch0Canceled(ObjectDragCanceled);
        }
    }

    private void MoveObject(Vector2 worldPos)
    {
        MoveObject(_pickObject, worldPos);
    }

    private void MoveObject(IBuildable obj, Vector2 worldPos)
    {
        var pos = Vector3Int.FloorToInt(worldPos);
        if (pos != _beforeCheckPos)
        {
            obj.Move(pos, pos - _player.CurrentMap.transform.position);
            _beforeCheckPos = pos;
            CheckValidation(obj);
        }
    }

    private void ObjectDragCanceled(Vector2 vector2, bool isPointerOverUI)
    {
        InputManager.Instance.UnregisterDragPerformed(MoveObject);
        InputManager.Instance.UnregisterTouch0Canceled(ObjectDragCanceled);

        if (_startPos == _pickObject.transform.position)
            return;
        if (canBuild == false)
            _pickObject.Move(_startPos, _startPos - _player.CurrentMap.transform.position);
        else
            PickCanceled?.Invoke(_pickObject, _startPos - _player.CurrentMap.transform.position);
    }

    private void StopDragMove(Vector2 screen, bool isPointerOverUI)
    {
        if (_screenDrag != null)
        {
            StopCoroutine(_screenDrag);
            _im.UnregisterTouch0Canceled(StopDragMove);
        }
    }

    private void CheckValidation(IBuildable clickable)
    {
        var map = _player.CurrentMap;
        bool result;
        if (clickable.IsBlocked == true)
        {
            result = map.CheckValidation(clickable.Size, clickable.Position);
            if (result == true)
            {
                result = map.CheckInteractionZone(clickable.Size, clickable.Position) == false;
            }
        }
        else
        {
            var ve = new Vector2(clickable.Size[0], clickable.Size[1]);
            result = map.IsGroundObjectNode(ve, clickable.Position);
        }

        clickable.SetCanBuildWithSpriteColor(result);
        canBuild = result;
    }
}
