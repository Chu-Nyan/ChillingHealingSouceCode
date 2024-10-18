using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBtn : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private TMP_Text _text;
    private int _index;

    private Action<int> _btnAction;

    public void Awake()
    {
        _btn.onClick.AddListener(() => _btnAction.Invoke(_index));
    }

    public void Init(int index, string text, Action<int> action)
    {
        _index = index;
        _text.text = text;
        _btnAction = action;
    }

}
