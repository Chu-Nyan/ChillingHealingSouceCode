using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IUnit, IBehaviorEvent, ITimeListener
{
    [SerializeField] private Transform _cameraArm;
    [SerializeField] private Transform _mainSprite;
    [SerializeField] private UnitUIController _unitUI;
    private PlayerData _data;
    private Inventory _inventory;

    private MapController _mapController;
    private List<GroundNode> _pathNodes;
    private Camping _currentEvent;
    private bool _isMoving;

    public event Action<bool> OnMovingChanged;
    public event Action<MissionType, int, int> BehaviorEvent;
    private event Action FixedUpdated;

    /* 팀원 코드
    public event Action<Direction> OnDirectionChanged;
    [SerializeField] private Character _character;
    private Direction _curDirection;
     */

    #region Property
    public Transform CameraArm
    {
        get => _cameraArm;
    }

    public Character Character
    {
        get => _character;
    }

    public UnitUIController UnitUI
    {
        get => _unitUI;
    }

    public PlayerData Data
    {
        get => _data;
    }

    public Inventory Inventory
    {
        get => _inventory;
    }

    public List<GroundNode> PathNodes
    {
        get => _pathNodes;
    }

    public Camping CurrentEvent
    {
        get => _currentEvent;
    }

    public Map CurrentMap
    {
        get => _mapController.CurrentMap;
    }

    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            if (_isMoving != value)
            {
                OnMovingChanged?.Invoke(value);
                _isMoving = value;
            }
        }
    }

    // 팀원 코드
    public Direction CurDirection
    {
        get => _curDirection;
        set
        {
            if (_curDirection != value)
            {
                OnDirectionChanged?.Invoke(value);
                _curDirection = value;
            }
        }
    }
    #endregion

    #region UnityMethod
    private void Awake()
    {
        _pathNodes = new(64);
        _data = new();
        // 인벤토리 초기화
        _inventory = new();
        _curDirection = Direction.Down;
        SaveManager.Instance._player = this;
    }

    private void FixedUpdate()
    {
        FixedUpdated?.Invoke();
    }
    #endregion

    #region Method
    public void Init(MapController mapController)
    {
        _mapController = mapController;
        SaveManager saveManager = SaveManager.Instance;
        int bonusSpace = 0;
        if (saveManager.IsSaveFileLoaded)
        {
            var save = saveManager._saveData.PlayerSaveData;
            Data.SetValueFromSaveFile(save);
            bonusSpace = save.BonusSpace;
        }
        _inventory.Init(Data.CampingCarLevel);
        _inventory.Space.AddBounsSpace(bonusSpace);
        _character.Init();
        TimeManager.Instance.RegisterHourListener(this);
    }

    public void Move(Vector3 worldPos)
    {
        Pathfinding.Instance.ChangePathNodeNonAlloc(_mapController.CurrentMap, transform.position, worldPos, _pathNodes);

        if (IsMoving == false && _pathNodes.Count != 0)
        {
            FixedUpdated += UpdateMove;
            IsMoving = true;
        }
    }

    public void StopMove()
    {
        FixedUpdated -= UpdateMove;
        IsMoving = false;
        _pathNodes.Clear();
    }

    private void UpdateMove()
    {
        Pathfinding.Instance.Move(this, _data.Speed);
        if (_pathNodes.Count == 0)
            StopMove();
        else // 팀원 코드
            CurDirection = ChangeDirection(_pathNodes[0]);
    }

    // 팀원 코드
    private Direction ChangeDirection(GroundNode node0)
    {
        var position = gameObject.transform.position;
        float dx = node0.WorldPosition.x - position.x;
        float dy = node0.WorldPosition.y - position.y;

        if (dx == 0)
            return dy > 0 ? Direction.Up : Direction.Down;
        return dx > 0 ? Direction.Right : Direction.Left;
    }


    public void MoveInsideAndOutside(MapCategory category)
    {
        StopMove();
        CancelInteraction();
        _mapController.ChangeMap(category, this);
        SoundManager.Instance.PlaySE(SoundType.DoorOpening);
    }

    public void MoveAnotherOutSideMap(MapType type)
    {
        if (type >= MapType.Car)
            return;

        StopMove();
        CancelInteraction();
        _mapController.ChangeMap(type, this);
    }

    public bool UpgradeCampingCar()
    {
        if (_data.CampingCarLevel >= PlayerData.MaxCampingCar)
            return false;

        _data.CampingCarLevel++;
        _mapController.UpgradeCampingCar(_data.CampingCarLevel);
        _inventory.SetLevel(_data.CampingCarLevel);
        return true;
    }

    public int AddItem(ItemType type, int stack)
    {
        var remain = _inventory.TryAdd(type, stack);
        BehaviorEvent?.Invoke(MissionType.CollectItem, (int)type, stack - remain);
        return remain;
    }

    public int UseItem(Consumable item, int amount)
    {
        int realityUse = amount / item.Effect.UsageAmount;
        if (realityUse == 0)
            return amount;

        BehaviorEvent?.Invoke(MissionType.CollectItem, (int)item.UniversalStatus.Type, -realityUse);
        BehaviorEvent?.Invoke(MissionType.UseItem, (int)item.UniversalStatus.Type, realityUse);
        var items = _inventory.FindItemsNonAlloc(item.UniversalStatus.Type);
        var reamin = amount;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (reamin != 0)
            {
                var consum = items[i] as Consumable;
                reamin -= consum.Use(this, _mapController.CurrentMap, amount);
            }
        }

        return reamin;
    }

    public void RemoveItem(ItemBase item, int amount)
    {
        BehaviorEvent?.Invoke(MissionType.CollectItem, (int)item.UniversalStatus.Type, -amount);
        Inventory.Remove(item.UniversalStatus.Type, amount);
    }

    public int InstanceUseItem(ItemType type, int amount)
    {
        BehaviorEvent?.Invoke(MissionType.UseItem, (int)type, amount);
        if (ItemGenerator.Instance.TryGetConsumeablerDB(type, out var data) == true)
        {
            Consumable.handler.Use(data, this, CurrentMap, amount);
        }

        return amount;
    }

    public void Interacte(Camping obj)
    {
        if (_currentEvent == obj)
            return;

        if (obj.Strategy is IBehaviorEvent camping)
        {
            camping.BehaviorEvent += OnPlayerBehaviorEvent;
        }

        if (obj.Strategy is ActiveCampingStrategy active && obj.Data.Level >= 1)
        {
            _unitUI.ShowEmoji(active.ActiveData.EmojiType, -1);
        }

        _currentEvent = obj;
        obj.StartAction(this);
    }

    public void SwtichSprite(bool isOn)
    {
        _mainSprite.gameObject.SetActive(isOn);
        if (isOn == false)
        {
            _unitUI.StopAllUI(true);
        }
    }

    public void CancelInteraction()
    {
        if (_currentEvent != null)
        {
            _currentEvent.Leave(this);
            if (_currentEvent.Strategy is IBehaviorEvent camping)
            {
                camping.BehaviorEvent -= OnPlayerBehaviorEvent;
            }

            _unitUI.HideEmoji(false);
            _currentEvent = null;
        }
    }

    public void GetTime(float time)
    {
        _data.HP -= 0.2f;
        _data.Cleanliness -= 0.2f;
    }

    public void Talk(DialogType type, bool isOverwrite, float time = 2)
    {
        // 다이얼로그 기능이 필요해지면 사용할 것
    }

    

    private void OnPlayerBehaviorEvent(MissionType missionType, int targetType, int amount)
    {
        BehaviorEvent?.Invoke(missionType, targetType, amount);
    }
    #endregion
}
