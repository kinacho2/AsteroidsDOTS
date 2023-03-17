using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerMoveComponent : IComponentData
{
    public float speed;
    public float rotationSpeedDeg;

}
