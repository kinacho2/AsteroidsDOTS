using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerInputComponent: IComponentData
{
    public Vector2 direction;
    public bool shoot;
}