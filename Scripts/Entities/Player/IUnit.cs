using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    public Transform transform { get; }
    public List<GroundNode> PathNodes { get; }
    public Camping CurrentEvent { get; }
    public UnitUIController UnitUI { get; }

    public void Move(Vector3 pos);
    public void Talk(DialogType type, bool isOverwrite, float time = 3);
    public void StopMove();
    public void MoveInsideAndOutside(MapCategory mapCategory);
    public void Interacte(Camping obj);
    public void CancelInteraction();
}
