using System;

public class UIOpenStrategy : ICampingStrategy
{
    private UnitUIController _uiController;
    private Action _uiOpen;
    private string _talkText;

    public void Init(Camping camping, CampingType type)
    {
        _uiController = camping.UIController;
        _talkText = default;
        if (type == CampingType.MissionBoard)
        {
            _talkText = "부탁이 생기면 적어둘게";
            _uiController.ShowEmoji(EmojiUI.EmojiType.Bang);
            _uiOpen += () =>
                {
                    UIManager.Instance.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.MissionBoard);
                };
        }
        else if (type == CampingType.ATM)
        {
            _uiController.ShowEmoji(EmojiUI.EmojiType.Trade);
            _uiOpen += () =>
            {
                UIManager.Instance.GetUI<CoinTradingUI>(UIName.CoinTradingUI);
            };
        }
    }

    public void Interact(Camping campingEvent, Player player)
    {
        _uiOpen?.Invoke();
    }

    public void Interact(Camping campingEvent, NPC npc)
    {
        if (_talkText != default)
        {
            npc.UnitUI.ShowBubble(_talkText,true);
        }
    }

    public void Leave(Camping campingEvent, Player player)
    {
    }

    public void Leave(Camping campingEvent, NPC npc)
    {
    }
}
