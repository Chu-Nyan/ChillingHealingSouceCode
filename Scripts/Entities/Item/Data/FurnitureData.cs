using UnityEngine;

public class FurnitureData
{
    public FurnitureUniversalData Universal;

    public Vector3 _position;
    public DirectionType Direction;

    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }

    public void Load(FurnitureSaveData save,FurnitureUniversalData universal)
    {
        Universal = universal;
        Position = new(save.PosX, save.PosY);
        Direction = save.Direction;
    }
}
