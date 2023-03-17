using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
[GenerateAuthoringComponent]
public struct PlayerInputComponent: IComponentData
{
    public float2 direction;
    public bool shoot;
}