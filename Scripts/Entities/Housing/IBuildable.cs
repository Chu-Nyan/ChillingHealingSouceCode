using UnityEngine;

public interface IBuildable
{
    public Transform transform { get; }
    public Vector3 Position { get; }
    public int[] Size { get; }
    public bool IsBlocked { get; }
    public bool CanRotation { get; }
    public void SetCanBuildWithSpriteColor(bool canBuild);
    public void Rotate(int dir);
    public void Move(Vector3 worldPos, Vector3 localPos);
    public void Destroy();
    public bool IsTouchOnSprite(Vector2 worldPos);
}
