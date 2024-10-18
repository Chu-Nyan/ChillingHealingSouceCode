using System;

public class PlayerData
{
    public enum PlayerState { Good, Bad, Zero }

    public static readonly float MaxHP = 100;
    public static readonly float MaxCleanliness = 100;
    public static readonly float DefaultSpeed = 6;
    public static readonly int MaxCampingCar = 3;

    public string Name = "첨지";
    public PlayerState HPState;
    private float _HP = MaxHP;
    public PlayerState CleanlinessState;
    private float _cleanliness = MaxCleanliness;
    private int _healingCoin = 500;
    private int _missionCoin = 5;
    public int CampingCarLevel = 1;

    public MapType OutsideMap = MapType.Forest;

    public Action<StatusType, float> OnStatusChanged;

    public float HP
    {
        get => _HP;
        set
        {
            value = Math.Clamp(value, 0, MaxHP);
            _HP = value;
            OnStatusChanged?.Invoke(StatusType.HP, _HP);
            SwitchHPState(value);
        }
    }

    public float Cleanliness
    {
        get => _cleanliness;
        set
        {
            value = Math.Clamp(value, 0, MaxCleanliness);
            _cleanliness = value;
            OnStatusChanged?.Invoke(StatusType.Cleanliness, _cleanliness);
            SwitchCleanlinessState(value);
        }
    }

    public int HealingCoin
    {
        get => _healingCoin;
        set
        {
            _healingCoin = value;
            OnStatusChanged?.Invoke(StatusType.HealingCoin, _healingCoin);
        }
    }

    public int MissionCoin
    {
        get => _missionCoin;
        set
        {
            _missionCoin = value;
            OnStatusChanged?.Invoke(StatusType.MissionCoin, _missionCoin);
        }
    }

    public float Speed
    {
        get
        {
            return DefaultSpeed - (HPState == PlayerState.Good ? 0 : (DefaultSpeed * 0.3f));
        }
    }

    public void Init(PlayerData Status)
    {
        HP = Status.HP;
        Cleanliness = Status.Cleanliness;
        CampingCarLevel = Status.CampingCarLevel;
        OutsideMap = Status.OutsideMap;
    }

    public void SetValueFromSaveFile(PlayerSaveData data)
    {
        Name = data.Name;
        HP = data.HP;
        Cleanliness = data.Cleanliness;
        HealingCoin = data.HealingCoin;
        MissionCoin = data.MissionCoin;
        CampingCarLevel = data.CampingCarLevel;
    }

    public int GetCoinAmount(CoinType type)
    {
        return type == CoinType.Healing ? _healingCoin : _missionCoin;
    }

    public void AddCoinAmount(CoinType type, int value)
    {
        if (type == CoinType.Healing)
            HealingCoin += value;
        else
            MissionCoin += value;
    }

    public bool CanPaid(CoinType type, int value)
    {
        return type switch
        {
            CoinType.Healing => _healingCoin >= value,
            CoinType.Mission => _missionCoin >= value,
            _ => false
        };
    }

    private void SwitchHPState(float hp)
    {
        PlayerState state;
        if (hp > 15)
            state = PlayerState.Good;
        else if (hp > 0)
            state = PlayerState.Bad;
        else
            state = PlayerState.Zero;

        if (state != HPState)
        {
            HPState = state;

            if (state == PlayerState.Bad)
            {
                var notiUI = UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI);
                notiUI.EnqueueText("체력이 낮아 패널티 효과가 적용됩니다.\r\n코인 수급량 50% 저하, 이동 속도 30% 저하");
                SoundManager.Instance.PlaySE(SoundType.HealthHygienePenaltyPopup);
            }
            else if (state == PlayerState.Zero)
            {
                var notiUI = UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI);
                notiUI.EnqueueText("체력이 낮아 힐링 활동이 불가합니다.\r\n체력 회복 후 힐링 활동을 즐겨보세요.");
                SoundManager.Instance.PlaySE(SoundType.HealthHygienePenaltyPopup);
            }
        }
    }

    private void SwitchCleanlinessState(float cleanliness)
    {
        PlayerState state;
        if (cleanliness > 10)
            state = PlayerState.Good;
        else if (cleanliness > 0)
            state = PlayerState.Bad;
        else
            state = PlayerState.Zero;
        if (state != CleanlinessState)
        {
            CleanlinessState = state;

            if (state == PlayerState.Bad)
            {
                UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI)
                    .EnqueueText("청결도가 낮아 패널티 효과가 적용됩니다.\r\nNPC 힐링 코인 획득량 50% 저하");
                SoundManager.Instance.PlaySE(SoundType.HealthHygienePenaltyPopup);
            }
            else if (state == PlayerState.Zero)
            {
                UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI)
                    .EnqueueText("청결도가 낮아 NPC가 씻는 것을 권유합니다.\r\n청결도 회복 후 함께 힐링 활동을 즐겨보세요.");
                SoundManager.Instance.PlaySE(SoundType.HealthHygienePenaltyPopup);
            }
        }
    }
}
