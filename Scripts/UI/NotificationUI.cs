using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : UIBase
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private Queue<NotificationData> _queue = new();
    private float _targetTime;
    private float _curTime;

    private void Update()
    {
        _curTime += Time.deltaTime;
        _canvasGroup.alpha = _curTime * 2;
        if (_curTime > _targetTime - 0.5)
            _canvasGroup.alpha = (_targetTime - _curTime) * 2;
        
        if (_curTime > _targetTime)
        {
            _queue.Dequeue();
            if (_queue.Count == 0)
            {
                Deactivate();
            }
            else
            {
                PeekNotification();
            }
        }
    }

    public void EnqueueText(string text, Sprite sprite = null, float time = 4)
    {
        _queue.Enqueue(new NotificationData(text, sprite, time));
        _image.enabled = sprite != null;
        if (_queue.Count == 1)
        {
            PeekNotification();
        }
    }

    private void PeekNotification()
    {
        _canvasGroup.alpha = 0;
        var data = _queue.Peek();
        _text.text = data.Text;
        _targetTime = data.Time;
        _curTime = 0;
        _image.sprite = data.Sprite;
    }
}

public class NotificationData
{
    public float Time;
    public string Text;
    public Sprite Sprite;

    public NotificationData(string text, Sprite sprite, float time)
    {
        Time = time;
        Text = text;
        Sprite = sprite;
    }
}
