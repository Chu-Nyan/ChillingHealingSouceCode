using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private Image _off;
    [SerializeField] private Image _on;
    [SerializeField] private RectTransform _ball;

    private Color ClearWhite = new(1, 1, 1, 0);
    private float OnBallPosition;
    private float OffBallPosition;
    private bool _isOn;
    
    private event Action<bool> ToggleClicked;

    private void Awake()
    {
        _btn.onClick.AddListener(OnToggleClicked);
        OnBallPosition = _ball.anchoredPosition.x;
        OffBallPosition = -OnBallPosition;
    }

    public void Init(Action<bool> callback)
    {
        ToggleClicked = callback;
    }

    public void SetToggle(bool isOn)
    {
        _isOn = isOn;
        InstanceChange(isOn);
    }

    private void OnToggleClicked()
    {
        _isOn = !_isOn;
        AnimationChange(_isOn);
        ToggleClicked?.Invoke(_isOn);
    }

    private void AnimationChange(bool isON)
    {
        if (isON == true)
        {
            _on.DOFade(1, 0.5f);
            _ball.DOLocalMoveX(OnBallPosition, 0.5f);
        }
        else
        {
            _on.DOFade(0, 0.5f);
            _ball.DOLocalMoveX(OffBallPosition, 0.5f);
        }
    }

    private void InstanceChange(bool isON)
    {
        if (isON == true)
        {
            _on.color = Color.white;
            _ball.anchoredPosition = new(OnBallPosition, 0);
        }
        else
        {
            _on.color = ClearWhite;
            _ball.anchoredPosition = new(OffBallPosition, 0);
        }
    }
}