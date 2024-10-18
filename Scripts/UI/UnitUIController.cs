using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitUIController : MonoBehaviour
{
    [SerializeField] private BubbleUI _bubble;
    [SerializeField] private EmojiUI _emoji;
    [SerializeField] private WorldNotificationUI _worldNotifiUI;

    public void StopAllUI(bool isHard)
    {
        HideEmoji(isHard);
        HideBubble();
    }

    public void ShowBubble(string text,bool isOverwrite, float showTime = 3)
    {
        _bubble.TalkBubble(text, isOverwrite, showTime);
    }

    public void ShowBubble(List<string> text, bool isOverwrite, float showTime)
    {
        _bubble.Talkbubble(text, false, showTime);
    }

    public void HideBubble()
    {
        _bubble.StopBubble();
    }

    public void ShowEmoji(EmojiUI.EmojiType type, float time = -1, Action action = null)
    {
        _emoji.Refresh(type, time, action);
    }

    public void HideEmoji(bool isHard)
    {
        _emoji.Hide(isHard);
    }

    public void ShowWorldNotifiUI(string text)
    {
        _worldNotifiUI.Refresh(text);
    }
}
