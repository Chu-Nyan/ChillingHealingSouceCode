using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    /* 팀원 코드
    public static bool TrySwitchStringToEnum<T>(string stringValue, out T result) where T : Enum
    {
        bool hasValue = Enum.IsDefined(typeof(T), stringValue);
        if (hasValue)
            result = (T)Enum.Parse(typeof(T), stringValue);
        result = default; // null ???
        return hasValue;
    }

    public static Vector3 RandomVector3InRange(Vector3 pivot, int range)
    {
        int x = (int)pivot.x + UnityEngine.Random.Range(-range, range);
        int y = (int)pivot.y + UnityEngine.Random.Range(-range, range);

        return new Vector3(x, y);
    }
    */

    public static Dictionary<K, V> NewEnumKeyDictionary<K, V>() where K : Enum where V : class, new()
    {
        Dictionary<K, V> dictionary = new();
        var enumArray = (K[])Enum.GetValues(typeof(K));

        for (int i = 0; i < enumArray.Length; i++)
        {
            K type = enumArray[i];
            dictionary.Add(type, new V());
        }

        return dictionary;
    }
}
