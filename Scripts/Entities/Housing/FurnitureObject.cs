using System;
using UnityEngine;

public enum DirectionType
{
    Bottom,
    Left,
    Top,
    Right,
}

public class FurnitureObject : MonoBehaviour, IBuildable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private FurnitureData _data;
    private int[] _size = new int[2];

    public event Action<FurnitureObject> Destroyed;

    public FurnitureData Data
    {
        get => _data;
    }

    public int[] Size
    {
        get => _size;
    }

    public bool IsBlocked
    {
        get => _data.Universal.IsBlocked;
    }
    public Vector3 Position 
    {
        get => _data.Position;
    }

    public bool CanRotation
    {
        get => _data.Universal.CanRotation;
    }

    public void Awake()
    {
        _data = new();
    }

    public void Init(Blueprint blueprint)
    {
        Move(blueprint.Position, blueprint.Position);
        _data.Universal = blueprint.Data.FurnitureData;
        _data.Direction = blueprint.Direction;
        Rotate(blueprint.Direction);
        _spriteRenderer.sortingLayerName = _data.Universal.IsBlocked == true ? Const.Layer_Character : Const.Layer_Ground;
    }

    public void Init(FurnitureData data)
    {
        _data = data;
        Move(data.Position, data.Position);
        Rotate(data.Direction);
        _spriteRenderer.sortingLayerName = _data.Universal.IsBlocked == true ? Const.Layer_Character : Const.Layer_Ground;
    }

    private void ChangeSpirte(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.transform.localPosition = (Vector2)sprite.bounds.size * 0.5f;
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
        Destroy(gameObject);
    }

    public bool IsTouchOnSprite(Vector2 worldPos)
    {
        return _spriteRenderer.bounds.Contains(worldPos);
    }

    public void SetCanBuildWithSpriteColor(bool canBuild)
    {
    }

    public void Rotate(int nextDir)
    {
        var direction = (DirectionType)(((int)_data.Direction + nextDir + 4) % 4);
        Rotate(direction);

    }

    public void Rotate(DirectionType direction)
    {
        Sprite sprite;

        if (direction == DirectionType.Bottom || direction == DirectionType.Top)
        {
            sprite = AssetManager.GetSpriteWithAtlas(Const.Sptire_Furnitures30000, _data.Universal.FrontSpritePath);
            if (sprite != null)
            {
                _size = _data.Universal.Size;
            }
        }
        else // Left, Right
        {
            sprite = AssetManager.GetSpriteWithAtlas(Const.Sptire_Furnitures30000, _data.Universal.SideSpritePath);
            if (sprite != null)
            {
                _size[0] = _data.Universal.Size[1];
                _size[1] = _data.Universal.Size[0];
            }
        }

        if (sprite != null)
        {
            _spriteRenderer.flipX = direction == DirectionType.Right || direction == DirectionType.Top;
            _data.Direction = direction;
            ChangeSpirte(sprite);
        }
    }

    public void Move(Vector3 worldPos, Vector3 localPos)
    {
        transform.position = worldPos;
        _data.Position = localPos;
    }
}
