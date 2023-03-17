using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInput_System : SystemBase
{
    float2 CameraLimits;
    protected override void OnCreate()
    {
        base.OnCreate();
        Camera camera = GameObject.FindObjectOfType<Camera>();

        var worldPoint = camera.ViewportToWorldPoint(Vector2.one);
        ;
        CameraLimits = new float2(worldPoint.y * camera.pixelWidth / camera.pixelHeight, worldPoint.y);
    }
    protected override void OnUpdate()
    {
        float tr = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        Entities.WithAll<PlayerInputComponent, PlayerMoveComponent>()
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex, ref PlayerInputComponent input, ref PlayerMoveComponent move) =>
                {
                    ref var dir = ref input.direction;
                    //move.speed = Mathf.Clamp(move.speed + tr * move.acceleration * deltaTime, -move.maxSpeed, move.maxSpeed);
                    move.cameraLimits = CameraLimits;
                    dir.y = tr;
                    dir.x = rot;
                })
                .Run();
    }
}
