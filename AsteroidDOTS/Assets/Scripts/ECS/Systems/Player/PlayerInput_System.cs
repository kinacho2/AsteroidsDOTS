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
            bool shoot = Input.GetKey(KeyCode.Space);
            Entities.WithAll<PlayerInputComponent, PlayerStatsComponent>()
                    .WithoutBurst()
                    .ForEach((Entity entity, int entityInQueryIndex, ref PlayerInputComponent input, ref PlayerStatsComponent move) =>
                    {
                        ref var dir = ref input.direction;
                        dir.y = tr;
                        dir.x = -rot;
                        input.shoot = shoot;
                    })
                    .Run();
        }
    }
}