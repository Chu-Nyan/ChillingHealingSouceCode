using UnityEngine;

public class Blueprint : MonoBehaviour, IBuildable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public Furniture Data;
    private bool _canBuild;
    private int[] _size = new int[2];

    private Vector3 _position;
    public DirectionType Direction;

    public bool CanBuild
    {
        get => _canBuild;
        set => _canBuild = value;
    }

    public int[] Size
    {
        get => _size;
    }

    public bool IsBlocked
    {
        get => Data.FurnitureData.IsBlocked;
    }

    public Vector3 Position
    {
        get => _position;
    }

    public bool CanRotation
    {
        get => Data.FurnitureData.CanRotation;
    }

    public bool IsTouchOnSprite(Vector2 worldPos)
    {
        return _spriteRenderer.bounds.Contains(worldPos);
    }

    public void Build()
    {
        if (CanBuild == false)
            return;
        CanBuild = false;
        Data.Stack--;
        Destroy();
    }

    public void Show(Furniture data)
    {
        gameObject.SetActive(true);
        Data = data;
        Direction = DirectionType.Bottom;
        _size[0] = data.FurnitureData.Size[0];
        _size[1] = data.FurnitureData.Size[1];
        Sprite sprite = AssetManager.GetSpriteWithAtlas(Const.Sptire_Furnitures30000, Data.FurnitureData.FrontSpritePath);
        SetCanBuildWithSpriteColor(true);
        SpriteChange(sprite);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void SetCanBuildWithSpriteColor(bool canBuild)
    {
        if (CanBuild == canBuild)
            return;

        CanBuild = canBuild;
        if (CanBuild == true)
        {
            _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
        else
            _spriteRenderer.color = new Color(1, 0.5f, 0.5f, 0.5f);
    }

    private void SpriteChange(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        if (sprite != null)
        {
            _spriteRenderer.transform.localPosition = (Vector2)sprite.bounds.size * 0.5f;
        }
        else
        {
            Debug.Log((int)Data.UniversalStatus.Type);
        }
    }

    public void Move(Vector3 worldPos, Vector3 localPos)
    {
        transform.position = worldPos;
        _position = localPos;
    }

    public void Rotate(int dir)
    {
        var direction = (DirectionType)(((int)Direction + dir + 4) % 4);
        Sprite sprite;

        if (direction == DirectionType.Bottom || direction == DirectionType.Top)
            sprite = AssetManager.GetSpriteWithAtlas(Const.Sptire_Furnitures30000, Data.FurnitureData.FrontSpritePath);
        else
        {
            sprite = AssetManager.GetSpriteWithAtlas(Const.Sptire_Furnitures30000, Data.FurnitureData.SideSpritePath);
        }

        if (sprite != null)
        {
            _spriteRenderer.flipX = Direction == DirectionType.Right || Direction == DirectionType.Top;
            var temp = _size[0];
            _size[0] = _size[1];
            _size[1] = temp;
            Direction = direction;
            SpriteChange(sprite);
        }
        else
        {
            Debug.Log(Data.FurnitureData.FrontSpritePath);
        }
    }
}
