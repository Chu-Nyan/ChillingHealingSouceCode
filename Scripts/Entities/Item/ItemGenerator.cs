using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Singleton<ItemGenerator>
{
    private readonly Dictionary<ItemCategory, ObjectPooling<ItemBase>> _items;
    private Dictionary<ItemType, ItemUniversalStatus> _universalDB;
    private Dictionary<ItemType, UseEffect> _consumeEffectDB;
    private Dictionary<ItemType, FurnitureUniversalData> _furnitureDB;
    private ItemBase _new;

    public event Action<ItemBase> Generated;

    public ItemGenerator()
    {
        _items = new()
        {
            { ItemCategory.Equipment, new (() => { return new Equipment(); }) },
            { ItemCategory.Consumable, new (() => { return new Consumable(); }) },
            { ItemCategory.Furniture, new (() => { return new Furniture(); }) },
            { ItemCategory.Resources, new (() => { return new Resource(); }) }
        };

        _universalDB = AssetManager.Instance.DeserializeJsonSync<Dictionary<ItemType, ItemUniversalStatus>>(Const.Json_ItemUniversal);
        _consumeEffectDB = AssetManager.Instance.DeserializeJsonSync<Dictionary<ItemType, UseEffect>>(Const.Json_ItemUseEffect);
        _furnitureDB = AssetManager.Instance.DeserializeJsonSync<Dictionary<ItemType, FurnitureUniversalData>>(Const.Json_FurnitureDB);
    }

    public ItemGenerator Ready(ItemType type)
    {
        var category = SwitchTypeToItemCategory(type);
        _new = _items[category].Dequeue();
        _new.Destroyed += (i) => _items[category].Enqueue(i);
        LoadUniversalData(type);

        return this;
    }

    public ItemGenerator SetStack(int value)
    {
        _new.Stack = Mathf.Min(value, _new.UniversalStatus.MaxStack);
        return this;
    }

    public ItemGenerator SetDestoryedEvent(Action<ItemBase> action)
    {
        _new.Destroyed += action;
        return this;
    }

    public ItemBase Generate()
    {
        Generated?.Invoke(_new);
        return _new;
    }

    public void Clear(ItemBase item)
    {
        var categoty = SwitchTypeToItemCategory(item.UniversalStatus.Type);
        _items[categoty].Enqueue(item);
    }

    private void LoadUniversalData(ItemType type)
    {
        var category = SwitchTypeToItemCategory(type);
        var universal = _universalDB[type];

        if (category == ItemCategory.Consumable)
        {
            var effect = _consumeEffectDB[type];
            var consum = _new as Consumable;
            consum.Init(universal, effect);
        }
        else if (category == ItemCategory.Furniture)
        {
            var data = _furnitureDB[type];
            var furniture = _new as Furniture;
            furniture.Init(universal, data);
        }
        else if (category == ItemCategory.Resources)
        {
            var resource = _new as Resource;
            resource.Init(universal);
        }
        else // category == ItemCategory.Equipment
        {
            var equipment = _new as Equipment;
            equipment.Init(universal);
        }
    }

    public static ItemCategory SwitchTypeToItemCategory(ItemType type)
    {
        int value = (int)type / 10000 - 1;
        return (ItemCategory)value;
    }

    public ItemUniversalStatus GetDB(ItemType type)
    {
        return _universalDB[type];
    }

    public bool TryGetFurnitureData(ItemType type,out FurnitureUniversalData data)
    {
        return _furnitureDB.TryGetValue(type,out data);
    }

    public bool TryGetConsumeablerDB(ItemType type, out UseEffect effect)
    {
        return _consumeEffectDB.TryGetValue(type, out effect);
    }

    // public Sprite GetSprite(CampingType type)
    // {
    //     var spriteAtlas = AssetManager.Instance.LoadAssetSync<SpriteAtlas>(Const.Sprite_Item);
    //     return spriteAtlas.GetSprite($"{type}");
    // }
}
