using System.Collections.Generic;

public class DialogSystem
{
    private readonly Dictionary<DialogType, Dialog> _dialogs = new();

    public void AddText(DialogType type,  string text, int textCase, int order = int.MaxValue)
    {
        if (_dialogs.TryGetValue(type, out var list) == false)
        {
            list = new();
            _dialogs.Add(type, list);
        }

        list.AddText(textCase,text,order);
    }

    public List<string> GetRandomText(DialogType type)
    {
        return _dialogs[type].GetRandom();
    }
}
