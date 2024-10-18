using System;
using UnityEngine;

public class Camping : MonoBehaviour
{
    [SerializeField] private UnitUIController _uiController;
    [SerializeField] private SpriteRenderer _mainSprite;
    private CampingData _data;
    private ICampingStrategy _strategy;
    private bool _isPlaying;

    public event Action<Camping> Destroyed;

    public UnitUIController UIController
    {
        get => _uiController;
    }

    public CampingData Data
    {
        get => _data;
    }

    public ICampingStrategy Strategy
    {
        get => _strategy;
        set => _strategy = value;
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => _isPlaying = value;
    }

    public void SetData(CampingData data)
    {
        _data = data;
        _data.Init();
        _data.Upgraded += Unlock;
    }

    public void Unlock(int level)
    {
        if (level >= 1 && _strategy is ActiveCampingStrategy active)
        {
            if (active.ActiveData.NPC != UnitType.None && active.NPC == null)
            {
                var center = (Vector2)transform.position + (Data.Universal.SizeVector * 0.5f) ;
                var npc = NPCGenerator.Instance.Ready(active.ActiveData.NPC)
                  .RandomPositionInRange(center, 2)
                  .SetPatrolPosition(center)
                  .Generate();

                active.NPC = npc;

                if (npc.Data.Level == 0)
                {
                    npc.Data.Upgrade();
                }
            }

            _data.Upgraded -= Unlock;
            var sprite = CampingGenerator.Instance.GetSprite(_data.Type);
            SetSprite(sprite);
        }
    }

    public void Init(ICampingStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        _data.Position = pos;
    }

    public void StartAction(Player player)
    {
        if (_data.Level > 0)
        {
            _isPlaying = true;
            _strategy.Interact(this, player);
        }
        else
        {
            var notiUI = UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI);
            notiUI.EnqueueText("캠핑 도구가 구비되어 있지 않아요 !\n캠핑 도구 도감에서 해금하세요.");
            player.UnitUI.ShowEmoji(EmojiUI.EmojiType.Disappointment);
            SoundManager.Instance.PlaySE(SoundType.HealthHygienePenaltyPopup);
        }
    }

    public void StartAction(NPC npc)
    {
        if (_data.Level > 0)
        {
            _strategy.Interact(this, npc);
        }
        else
        {
            npc.UnitUI.ShowBubble("다음에 다시 오자", true);
        }
    }

    public void Leave(Player player)
    {

        if (_data.Level > 0)
        {
            IsPlaying = false;
            _strategy.Leave(this, player);
        }
        else
        {
            player.UnitUI.ShowBubble("다음에 다시오자", true);
        }
    }

    public void Leave(NPC npc)
    {
        if (_data.Level > 0)
        {
            _strategy.Leave(this, npc);
        }
    }

    public void Destory()
    {
        _isPlaying = false;
        _strategy = null;
        _uiController.StopAllUI(true);
        gameObject.SetActive(false);
        Destroyed?.Invoke(this);
        Destroyed = null;
    }

    public void SetSprite(Sprite sprite)
    {
        _mainSprite.sprite = sprite;
        if (sprite != null)
        {
            _mainSprite.sortingLayerName = _data.Universal.IsBlocked == true ? Const.Layer_Character : Const.Layer_Ground;
            var size = (Vector2)sprite.bounds.size;
            size.x *= 0.5f;
            _uiController.transform.localPosition = size;
        }
    }
}
