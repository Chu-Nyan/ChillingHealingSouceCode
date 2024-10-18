using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BubbleUI : UIBase
{
    [SerializeField] private TMP_Text _text;

    private readonly Queue<string> _queue = new();
    private float _showTime = 3;
    private float _time;

    public void Update()
    {
        _time += Time.deltaTime;
        if (_time > _showTime)
        {
            _queue.Dequeue();
            if (_queue.Count == 0)
            {
                StopBubble();
            }
            else
            {
                PeekText();
            }
        }
    }

    public void TalkBubble(string text, bool isOverwrite, float showTime = 3)
    {
        if (text == default || text == null)
            return;
        if (isOverwrite == true)
            _queue.Clear();

        _queue.Enqueue(text);
        _showTime = showTime;

        if (_queue.Count == 1)
            PeekText();

        Activate();
    }

    public void Talkbubble(List<string> texts, bool isOverwrite, float showTime = 3)
    {
        if (texts.Count == 0)
            return;

        if (isOverwrite == true)
            _queue.Clear();

        foreach (var item in texts)
        {
            _queue.Enqueue(item);
        }
        _showTime = showTime;

        if (_queue.Count == texts.Count)
            PeekText();

        Activate();
    }

    public void StopBubble()
    {
        _time = 0;
        _queue.Clear();
        Deactivate();
    }

    public void EnqueueText(string text)
    {
        _queue.Enqueue(text);
        if (_queue.Count == 1)
        {
            PeekText();
        }
    }

    private void PeekText()
    {
        var text = _queue.Peek();
        _text.text = text;
        _time = 0;
    }
}
