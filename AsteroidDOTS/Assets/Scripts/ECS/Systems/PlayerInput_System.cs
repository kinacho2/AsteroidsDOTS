using Asteroids.ECS.Components;
using Asteroids.Setup;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class PlayerInput_System : SystemBase
    {
        protected override void OnUpdate()
        {
            float tr = Input.GetAxis("Vertical");
            float rot = Input.GetAxis("Horizontal");

            Entities.WithAll<PlayerInputComponent, PlayerMoveComponent>()
                    .WithoutBurst()
                    .ForEach((Entity entity, int entityInQueryIndex, ref PlayerInputComponent input, ref PlayerMoveComponent move) =>
                    {
                        ref var dir = ref input.direction;
                        move.cameraLimits = Configs.CameraLimits;
                        dir.y = tr;
                        dir.x = -rot;
                    })
                    .Run();
        }
    }
}