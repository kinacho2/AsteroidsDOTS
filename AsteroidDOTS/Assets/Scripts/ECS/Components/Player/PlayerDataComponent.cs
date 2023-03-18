using Unity.Entities;

public struct PlayerDataComponent : IComponentData
{
    public float maxSpeed;
    public float acceleration;
    public float rotationSpeedDeg;
    public float restitution;
    public float stunnedTime;
}
