using System.Collections.Generic;

public class Dialog
{
    private readonly Dictionary<int, List<string>> _textList = new();

    public void AddText(int textCase, string text, int order)
    {
        if (_textList.TryGetValue(textCase, out var list) == false)
        {
            list = new();
            _textList.Add(textCase, list);
        }

        if (_textList.Count <= order)
        {
            list.Add(text);
        }
        else
        {
            list.Insert(order, text);
        }
    }

    public List<string> GetRandom()
    {
        var index = UnityEngine.Random.Range(0, _textList.Count);
        return _textList[index];
    }
}
