using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapSlotUI : UIBase
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private MapType _mapType;
    private Action<MapType> _clickEvent;

    private void Awake()
    {
        _button.onClick.AddListener(OnClicked);
    }

    public void Init(MapType mapType, Action<MapType> action)
    {
        _mapType = mapType;
        _clickEvent = action;
        string text = "~ 추가 예정 ~";

        switch (mapType)
        {
            case MapType.Forest:
                text = "숲 속 마을로 가기";
                break;
            case MapType.Sea:
                text = "바다 마을로 가기";
                break;
        }

        _text.text = text;
    }

    private void OnClicked()
    {
        _clickEvent?.Invoke(_mapType);
    }
}
