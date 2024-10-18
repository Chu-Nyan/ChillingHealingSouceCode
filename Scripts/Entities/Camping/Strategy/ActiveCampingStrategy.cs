using System;

public class ActiveCampingStrategy : ICampingStrategy, IBehaviorEvent, ITimeListener
{
    private static readonly float RewardTime = 4;  // 획득 타임 : 1 * 60초

    private CampingData _data;
    private ActiveCampingData _activeData;
    private Player _player;
    private NPC _subNPC;

    private bool _isNPCPlaying;
    private bool _isTalked;
    private int _tryCount;
    private float _time;
    private int _saveReward;

    public event Action<MissionType, int, int> BehaviorEvent;

    public ActiveCampingData ActiveData
    {
        get => _activeData;
    }

    public NPC NPC
    {
        get => _subNPC;
        set => _subNPC = value;
    }

    public void Init(CampingData data)
    {
        _data = data;
        _activeData = data.Universal as ActiveCampingData;
    }

    public void Interact(Camping campingEvent, Player player)
    {
        if (player.Data.HP - _activeData.DecreaseHp < 0 && _activeData.DecreaseHp > 0)
        {
            UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI)
                .EnqueueText("체력이 낮아 힐링 활동이 불가합니다.\r\n체력 회복 후 힐링 활동을 즐겨보세요.");
            player.CancelInteraction();
        }
        else
        {
            TimeManager.Instance.RegisterListener(this);
            _player = player;
            _isTalked = false;
            _time = 0;
            _tryCount = 0;
            if (_saveReward != 0)
            {
                _player.Data.AddCoinAmount(CoinType.Healing, _saveReward);
                _subNPC.Talk(DialogType.GiveCoin, true);
                player.UnitUI.ShowWorldNotifiUI($"{Const.TextEmoji_HealingCoin} {_saveReward}");
                _saveReward = 0;
            }
            else if (_activeData.NPC != UnitType.None)
            {
                _subNPC.Talk(DialogType.InActiveCamping, false);
            }
        }

        SoundManager.Instance.PlayBGM(_activeData.SoundPath);
    }

    public void Interact(Camping campingEvent, NPC npc)
    {
        if (npc.Data.Type != _activeData.NPC)
            return;

        _isNPCPlaying = true;
        if (campingEvent.IsPlaying == false)
        {
            _subNPC.Talk(DialogType.SoloActiveCamping, true);
            if (npc.IsSetCamping == false)
            {
                _saveReward += npc.Data.CurrentLevel.Coin;
            }
        }
    }

    public void Leave(Camping campingEvent, Player player)
    {
        TimeManager.Instance.UnregisterListener(this);
        if (player.Data.HP < _activeData.DecreaseHp)
            player.UnitUI.ShowBubble("지쳤어...", true);
        else
            player.UnitUI.ShowBubble("정말 좋았어!!", true);

        if (_activeData.NPC != UnitType.None)
        {
            _subNPC.Talk(DialogType.OutActiveCamping, true);
        }
        if (_activeData.SoundPath != SoundType.None)
        {
            player.CurrentMap.TryPlayMapThemeMusic();
        }
    }

    public void Leave(Camping campingEvent, NPC npc)
    {
        if (_subNPC == npc)
        {
            npc.UnitUI.ShowBubble("재밌군요!", true);
            _isNPCPlaying = false;
        }
        else
        {
            npc.UnitUI.ShowBubble("나랑은 맞지 않았어...", true);
        }
    }

    public void GetTime(float time)
    {
        CheakTime(time, _player);
    }

    private void InjoyCamping(Player player)
    {
        if (player.Data.HP - _activeData.DecreaseHp < 0)
        {
            player.CancelInteraction();
        }
        else
        {
            ReceiveItemWithRandom(player);
            ProcessGetCoin(player);
            BehaviorEvent?.Invoke(MissionType.VisitCamping, (int)_data.Type, 1);
        }
    }

    private void CheakTime(float time, Player player)
    {
        _time += time;

        if (RewardTime <= _time)
        {
            InjoyCamping(player);
            _time = 0;
        }
    }

    private void TryTalk()
    {
        if (_isTalked == true)
            return;

        _tryCount++;
        if (_tryCount > 4)
        {
            _subNPC.TalkEvent(DialogType.ActiveHealing, FinishAction);
            _tryCount = 0;
            _isTalked = true;
            SoundManager.Instance.PlaySE(SoundType.Bang);
        }

        void FinishAction()
        {
            BehaviorEvent?.Invoke(MissionType.TalkNPC, (int)_subNPC.Data.Type, 1);
        }
    }

    private void ProcessGetCoin(Player player)
    {
        var data = player.Data;
        int value = _data.CurrentLevel.Coin;

        if (_isNPCPlaying == true)
        {
            if (data.CleanlinessState == PlayerData.PlayerState.Good)
                value += _subNPC.Data.CurrentLevel.Coin;
            else if (data.CleanlinessState == PlayerData.PlayerState.Bad)
                value += (int)(_subNPC.Data.CurrentLevel.Coin * 0.5f);

            BehaviorEvent?.Invoke(MissionType.VisitCampingTogether, (int)_activeData.NPC, 1);
            TryTalk();
        }
        if (data.HPState == PlayerData.PlayerState.Bad)
        {
            value = (int)(value * 0.5f);
        }

        data.HP -= _activeData.DecreaseHp;
        data.Cleanliness -= _activeData.DecreaseCleanliness;
        player.Data.HealingCoin += value;
        if (value != 0)
        {
            player.UnitUI.ShowWorldNotifiUI($"{Const.TextEmoji_HealingCoin} {value}");
        }
    }

    private void ReceiveItemWithRandom(Player player)
    {
        if (_activeData.MissionItem == ItemType.None)
            return;

        if (UnityEngine.Random.Range(0, 10) < 1)
        {
            player.AddItem(_activeData.MissionItem, 1);
        }
    }
}
