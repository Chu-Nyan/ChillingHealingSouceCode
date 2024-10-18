using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private readonly PlayerInputSystem _inputSystem;
    public readonly PlayerInputSystem.InGameActions Ingame;

    private PointEventRayCastHelper _rayHelper;
    private Camera _cam;

    private event Action<Vector2, bool> Touch0Performed;
    private event Action<Vector2, bool> Touch0Canceled;
    private event Action Touch1Started;
    private event Action Touch1Canceled;

    private event Action<Vector2> DragPerformed;

    public InputManager()
    {
        _inputSystem = new();
        Ingame = _inputSystem.InGame;
        InitRegisterKeyMap();
    }

    public void Init()
    {
        _cam = Camera.main;
        _rayHelper = PointEventRayCastHelper.Instance;
    }

    public void Enable()
    {
        _inputSystem.Enable();
    }

    public void Disable()
    {
        _inputSystem.Disable();
    }

    private void InitRegisterKeyMap()
    {
        Ingame.Click.performed += OnTouch0Performed;
        Ingame.Click.canceled += OnTouch0Canceled;
        Ingame.Touch1Contact.started += OnTouch1Started;
        Ingame.Touch1Contact.canceled += OnTouch1Canceled;
        Ingame.Drag.performed += OnDragPerformed;
    }

    private void OnTouch0Performed(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        var clickPos = Mouse.current.position.ReadValue();
#elif UNITY_ANDROID
        var clickPos = Touchscreen.current.position.ReadValue();
#else // 소스 코드
        var clickPos = new Vector3();
#endif

        var isPointerOverUI = _rayHelper.RayCastAll(clickPos).Count > 0;
        Touch0Performed?.Invoke(clickPos, isPointerOverUI);
    }

    private void OnTouch0Canceled(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        var clickPos = Mouse.current.position.ReadValue();
#elif UNITY_ANDROID
        var clickPos = Touchscreen.current.position.ReadValue();
#else // 소스 코드
        var clickPos = new Vector3();
#endif

        var isPointerOverUI = _rayHelper.RayCastAll(clickPos).Count > 0;
        Touch0Canceled?.Invoke(clickPos, isPointerOverUI);
    }

    private void OnTouch1Started(InputAction.CallbackContext context)
    {
        Touch1Started?.Invoke();
    }

    private void OnTouch1Canceled(InputAction.CallbackContext context)
    {
        Touch1Canceled?.Invoke();
    }

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        DragPerformed?.Invoke(_cam.ScreenToWorldPoint(context.ReadValue<Vector2>()));
    }

    public void RegisterTouch0Performed(Action<Vector2, bool> action)
    {
        Touch0Performed += action;
    }

    public void RegisterTouch0Canceled(Action<Vector2, bool> action)
    {
        Touch0Canceled += action;
    }

    public void RegisterTouch1Started(Action action)
    {
        Touch1Started += action;
    }

    public void RegisterTouch1Canceled(Action action)
    {
        Touch1Canceled += action;
    }

    public void RegisterDragPerformed(Action<Vector2> action)
    {
        DragPerformed += action;
    }

    public void UnregisterTouch0Performed(Action<Vector2,bool> action)
    {
        Touch0Performed -= action;
    }

    public void UnregisterTouch0Canceled(Action<Vector2, bool> action)
    {
        Touch0Canceled -= action;
    }

    public void UnregisterTouch1Started(Action action)
    {
        Touch1Started -= action;
    }

    public void UnregisterTouch1Canceled(Action action)
    {
        Touch1Canceled -= action;
    }

    public void UnregisterDragPerformed(Action<Vector2> action)
    {
        DragPerformed -= action;
    }
}
