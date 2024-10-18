using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteBtn : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private Image _image; 
    private int _index;

    private Action<int> _action;

    private void Awake()
    {
        _btn.onClick.AddListener(() => _action?.Invoke(_index));
        _image.preserveAspect = true;
    }

    public void Init(Action<int> action)
    {
        _action = action;
    }

    public void SetSlotData(int index, Sprite sprite)
    {
        _index = index;
        if (sprite != null)
            _image.sprite = sprite;
        _image.color = new Color(1, 1, 1, sprite == null ? 0 : 1);
    }
}