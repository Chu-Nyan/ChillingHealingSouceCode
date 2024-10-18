using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickHandler : MonoBehaviour, IBehaviorEvent, IClickHandler
{
    private enum Mode { Move, NPC, Zoom, }
    private static readonly float _zoomSpeed = 0.75f;

    private CameraController _camera;
    private Mode _mode;
    private Player _player;
    private NPC _clickNPC;
    private Vector3 _startPos;
    private bool _isCarring;

    private Coroutine _zoom;

    public event Action<MissionType, int, int> BehaviorEvent;

    public ClickMode ClickMode
    {
        get => ClickMode.Player;
    }

    public void Init(Player player, CameraController camera)
    {
        _player = player;
        _camera = camera;
    }

    public void Register()
    {
        InputManager.Instance.RegisterTouch0Performed(Click);
        InputManager.Instance.RegisterTouch1Started(StartZoom);
        InputManager.Instance.RegisterTouch1Canceled(CancelZoom);
        InputManager.Instance.RegisterTouch0Canceled(CancelClick);
    }

    public void Unregister()
    {
        InputManager.Instance.UnregisterTouch0Performed(Click);
        InputManager.Instance.UnregisterTouch1Started(StartZoom);
        InputManager.Instance.UnregisterTouch1Canceled(CancelZoom);
        InputManager.Instance.UnregisterTouch0Canceled(CancelClick);
        CancelZoom();
    }

    private void Click(Vector2 pos, bool isPointerOverUI)
    {
        if (isPointerOverUI == true)
            return;

        pos = Camera.main.ScreenToWorldPoint(pos);
        if (ContainNPCFromClickPostion(pos) == true)
        {
            _clickNPC.SwitchNPCAI(false);
            _isCarring = true;
            _mode = Mode.NPC;
#if UNITY_EDITOR
            DragNPC(Camera.main.ScreenToWorldPoint(Mouse.current.position.value));
#elif UNITY_ANDROID
        DragNPC(Camera.main.ScreenToWorldPoint(Touchscreen.current.position.value));
#endif
            InputManager.Instance.RegisterDragPerformed(DragNPC);
        }
        else
        {
            _mode = Mode.Move;
        }
    }

    private void CancelClick(Vector2 screenPos, bool isPointerOverUI)
    {
        if (_mode == Mode.NPC)
        {
            if (_isCarring == true)
            {
                _isCarring = false;
                CancelDragNPC();
            }

            _clickNPC.SwitchNPCAI(true);
        }
        else if (_mode == Mode.Move && isPointerOverUI == false)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            _player.Move(worldPos);
        }
        else if (_mode == Mode.Zoom)
        {
            CancelZoom();
        }
    }

    private void StartZoom()
    {
        if (_isCarring == true)
            return;

        _mode = Mode.Zoom;
        _zoom = StartCoroutine(Zoom());
    }

    private IEnumerator Zoom()
    {
        var im = InputManager.Instance;
        var prePos0 = im.Ingame.Drag.ReadValue<Vector2>();
        var prePos1 = im.Ingame.Touch1Position.ReadValue<Vector2>();
        var preDistance = (prePos0 - prePos1).magnitude;
        yield return null;

        while (true)
        {
            var currentPos0 = im.Ingame.Drag.ReadValue<Vector2>();
            var currentPos1 = im.Ingame.Touch1Position.ReadValue<Vector2>();
            var distance = (currentPos0 - currentPos1).magnitude;

            var zoomAmount = (preDistance - distance);

            var next = math.lerp(_camera.Distance, _camera.Distance + zoomAmount, _zoomSpeed * Time.deltaTime);
            _camera.SetDistance(next);
            preDistance = distance;
            yield return null;
        }
    }

    private void DragNPC(Vector2 pos)
    {
        _clickNPC.transform.position = pos;
        _clickNPC.IsDragging = true;
    }

    private void CancelDragNPC()
    {
        InputManager.Instance.UnregisterDragPerformed(DragNPC);
        _clickNPC.IsDragging = false;
        if (_clickNPC.Map.IsMoveableNode(_clickNPC.transform.position))
        {
            var map = _clickNPC.Map;
            int x = (int)_clickNPC.transform.position.x;
            int y = (int)_clickNPC.transform.position.y;
            if (map.Nodes[x, y].CanInteraction
             && map.Nodes[x, y].Interaction.Data.Universal is ActiveCampingData active)
            {
                if (active.NPC == _clickNPC.Data.Type)
                {
                    _clickNPC.IsSetCamping = true;
                }
            }
            else if (map.Nodes[x, y].CanInteraction == false)
            {
                _clickNPC.UnitUI.ShowBubble("여긴 어디지 ?", true);
            }

            _clickNPC.Move(_clickNPC.transform.position);
        }
        else
        {
            _clickNPC.transform.position = _startPos;
        }
    }

    private bool ContainNPCFromClickPostion(Vector2 pos)
    {
        foreach (var item in _player.CurrentMap.NPCs)
        {
            var bound2d = item.SpriteRenderer.bounds;
            bound2d.center = (Vector2)bound2d.center;

            if (bound2d.Contains(pos) == false)
                continue;

            _startPos = item.transform.position;
            _clickNPC = item;
            return true;
        }

        return false;
    }

    private void CancelZoom()
    {
        if (_zoom != null)
        {
            StopCoroutine(_zoom);
        }
    }
}
