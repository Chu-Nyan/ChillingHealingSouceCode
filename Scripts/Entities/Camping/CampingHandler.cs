using UnityEngine;

public class CampingHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private CampingType _type;
    [SerializeField] private Vector2 _interactionOffset;
    [SerializeField] private Vector2 _interactionSize;
    [SerializeField] private Vector2 _campingSize;

    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
    }

    public CampingType Type
    {
        get => _type;
    }

    public Rect InteractRect
    {
        get
        {
            var pos = transform.position + (Vector3)_interactionOffset;
            return new(pos, _interactionSize); 
        }

    }

    public Rect PositionSize
    {
        get => new(transform.position, _campingSize);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Vector4(255, 0, 0, 0.5f);
        Gizmos.DrawCube(InteractRect.center, InteractRect.size);
        Gizmos.DrawCube(PositionSize.center, PositionSize.size);
    }
}
