//using System;
//using System.Collections.Generic;

//[Serializable]
//public class CollectionTheme<T> where T : Enum
//{
//    public readonly int ID;
//    public readonly List<Collection<T>> Items;
//    private bool _isAllUnlocked;
//    private int _unlockCount;

//    public bool IsAllUnlocked => _isAllUnlocked;

//    public CollectionTheme(int id)
//    {
//        ID = id;
//        Items = new();
//    }

//    public void Unlock(T type)
//    {
//        foreach (var item in Items)
//        {
//            int result = type.CompareTo(item.Item);

//            if (result < 0)
//                break;
//            if (result == 0)
//            {
//                item.Unlock(true);
//                _unlockCount++;
//                break;
//            }
//        }

//        _isAllUnlocked = _unlockCount >= Items.Count;
//    }

//    public void Add(Collection<T> Collection)
//    {
//        Items.Add(Collection);
//    }
//}
