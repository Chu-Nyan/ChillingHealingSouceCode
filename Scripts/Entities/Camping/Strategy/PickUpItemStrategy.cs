public class PickUpItemStrategy : ICampingStrategy
{
    private ItemType _type;
    private int _amount;
    private bool _isEmpty;

    public void Init(ItemType type, int amount)
    {
        _type = type;
        _amount = amount;
        _isEmpty = false;
    }

    public void Interact(Camping campingEvent, Player player)
    {
        player.UnitUI.ShowEmoji(EmojiUI.EmojiType.Bang, -1, GetItem);
        SoundManager.Instance.PlaySE(SoundType.Bang);
        void GetItem()
        {
            _isEmpty = true;
            var notiUI = UIManager.Instance.GetUI<NotificationUI>(UIName.NotificationUI);
            SoundManager.Instance.PlaySE(SoundType.CloverAcquiredPopup);
            if (_type == ItemType.ThreeLeafClover)
            {
                player.UnitUI.ShowEmoji(EmojiUI.EmojiType.ThreeClover);
                notiUI.EnqueueText("웃어요 ! 오늘은 행복이 가득한 날 !\n세잎클로버를 획득했어요 !",
                    AssetManager.GetItemIconSprite(ItemType.ThreeLeafClover));
            }
            else
            {
                player.UnitUI.ShowEmoji(EmojiUI.EmojiType.FourClover);
                notiUI.EnqueueText("우와 ! 오늘은 행운이 가득한 날 !\n네잎클로버를 획득했어요 !",
                    AssetManager.GetItemIconSprite(ItemType.FourLeafClover));
            }

            player.AddItem(_type, _amount);
        }
    }

    public void Interact(Camping campingEvent, NPC npc)
    {
        npc.UnitUI.ShowBubble("여기 뭔가 있는 거 같아", true);
    }

    public void Leave(Camping campingEvent, Player player)
    {
        player.UnitUI.HideEmoji(false);
        if (_isEmpty == true)
        {
            campingEvent.Destory();
        }
    }

    public void Leave(Camping campingEvent, NPC npc)
    {
    }
}
