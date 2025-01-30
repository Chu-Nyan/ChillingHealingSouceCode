using ChillingHealing.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IUnit
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private UnitUIController _unitUI;
    private NPCAI _aiHandler;
    private BehaviorAI<NPC> _ai;
    private DialogSystem _dialog;
    private NPCData _data;

    private Map _map;
    private List<GroundNode> _paths;
    private bool _isMoveing;
    private bool _isDragging;


    private Vector3 _patrolPosition;
    private Camping _currentEvent;
    private bool _isSetCamping;

    private event Action FixedUpdated;
    public Action<bool> OnMovingChanged;
    public Action<bool> OnDragChanged;
    public Action<NPC> OnDestroyed;

    /* 팀원 코드
    [SerializeField] private NPCCharacter _character;
    public Action<Direction> OnDirectionChanged;
    private Direction _curDirection;
    */

    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
    }

    public NPCData Data
    {
        get => _data;
    }

    public Map Map
    {
        get => _map;
    }

    public List<GroundNode> PathNodes
    {
        get => _paths;
    }

    public bool IsMoveing
    {
        get => _isMoveing;
        set
        {
            if (_isMoveing != value)
            {
                OnMovingChanged?.Invoke(value);
                _isMoveing = value;
            }
        }
    }

    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            if (_isDragging != value)
            {
                OnDragChanged?.Invoke(value);
                _isDragging = value;
            }
        }
    }

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

    public Vector3 PatrolPosition
    {
        get => _patrolPosition;
        set => _patrolPosition = value;
    }

    public Camping CurrentEvent 
    { 
        get => _currentEvent; 
    }

    public bool IsSetCamping
    {
        get => _isSetCamping;
        set => _isSetCamping = value;
    }

    public UnitUIController UnitUI
    {
        get => _unitUI;
    }

    public DialogSystem DialogSystem
    {
        get => _dialog;
    }

    private void Awake()
    {
        _paths = new List<GroundNode>(16);
        _aiHandler = new();
        _aiHandler.SetHealingNPC();
        _ai = _aiHandler.AI;
        _ai.IsRunning = true;
        _curDirection = Direction.Down;
    }

    private void FixedUpdate()
    {
        _ai.Execute(this);
        FixedUpdated?.Invoke();
    }

    public void InitStatus(NPCData universal, DialogSystem dialog)
    {
        _data = universal;
        _dialog = dialog;
        _character.Init();
        var pos = _spriteRenderer.sprite.bounds.size * 0.6f;
        _unitUI.transform.localPosition = pos;
    }

    public void InitMap(Map handler)
    {
        _map = handler;
    }

    public void Move(Vector3 pos)
    {
        Pathfinding.Instance.ChangePathNodeNonAlloc(_map, transform.position, pos, _paths);
        if (_isMoveing == false && _paths.Count != 0)
        {
            IsMoveing = true;
            FixedUpdated += UpdateMove;
        }
    }

    public void StopMove()
    {
        if (_isSetCamping == true && _currentEvent != null)
        {
            CurDirection = ChangeDirection(_currentEvent.transform.position);
        }
        IsMoveing = false;
        FixedUpdated -= UpdateMove;
        _paths.Clear();
    }

    private void UpdateMove()
    {
        Pathfinding.Instance.Move(this, NPCData.Speed);
        if (_paths.Count == 0)
            StopMove();
        else
            CurDirection = ChangeDirection(new Vector3(_paths[0].WorldPosition.x, _paths[0].WorldPosition.y));
    }

    // 팀원 코드
    private Direction ChangeDirection(Vector3 dirPos)
    {
        var position = gameObject.transform.position;
        float dx = dirPos.x - position.x;
        float dy = dirPos.y - position.y;
        if (dx == 0)
            return dy > 0 ? Direction.Up : Direction.Down;
        return dx > 0 ? Direction.Right : Direction.Left;
    }

    public void Interacte(Camping obj)
    {
        _currentEvent = obj;

        if (_currentEvent.Data.Universal.InteractionType == InteractionType.Running)
        {
            obj.StartAction(this);
            _unitUI.ShowEmoji(EmojiUI.EmojiType.Heart);
        }
        else
        {
            obj.StartAction(this);
        }
    }

    public void CancelInteraction()
    {
        if (_currentEvent.Data.Universal.InteractionType == InteractionType.Running)
        {
            _currentEvent.Leave(this);
            _currentEvent = null;
            _unitUI.HideEmoji(false);
            _isSetCamping = false;
        }
    }

    public void RunInteractionAction()
    {
        _currentEvent.StartAction(this);
    }

    public void Talk(DialogType type, bool isOverwrite, float time = 4)
    {
        var texts = _dialog.GetRandomText(type);
        _unitUI.ShowBubble(texts, isOverwrite, time);
    }

    public void TalkEvent(DialogType type, Action finishAction = null)
    {
        _unitUI.ShowEmoji(EmojiUI.EmojiType.Bang, 5, () =>
        {
            Talk(type, true);
            if (_isSetCamping == true)
            {
                _unitUI.ShowEmoji(EmojiUI.EmojiType.Heart);
                finishAction?.Invoke();
            }
        });
    }

    public void SwitchNPCAI(bool isOn)
    {
        if (isOn == true)
        {
            _ai.IsRunning = true;
        }
        else
        {
            _ai.IsRunning = false;
            StopMove();
        }
    }

    public void Destroy()
    {
        _unitUI.StopAllUI(true);
        StopMove();
        gameObject.SetActive(false);
        OnDestroyed?.Invoke(this);
        OnDestroyed = null;
    }

    public void MoveInsideAndOutside(MapCategory mapCategory)
    {

    }
}
