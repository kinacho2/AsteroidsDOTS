using Asteroids.ECS.Components;
using Asteroids.Setup;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class PlayerInput_System : SystemBase
    {
        private const float INPUT_THRESHOLD = 0.001f;

        protected override void OnUpdate()
        {
            float tr = Input.GetAxis("Vertical");
            float rot = Input.GetAxis("Horizontal");
            bool shoot = Input.GetKey(KeyCode.Space);

            Entities.WithAll<ShipInputComponent, ShipStatsComponent, PlayerComponent>()
                    .WithoutBurst()
                    .ForEach((Entity entity, int entityInQueryIndex, 
                        ref ShipInputComponent input, 
                        ref ShipStatsComponent stats) =>
                    {
                        ref var dir = ref input.direction;

                        if (dir.y != tr)
                        {
                            if (math.abs(tr) > math.abs(dir.y))
                                Events_System.OnPlayerStartMove.PostEvent(new Events.PlayerMove());
                            else if(math.abs(tr) < math.abs(dir.y))
                                Events_System.OnPlayerStopMove.PostEvent(new Events.PlayerMove());

                        }

                        dir.y = tr;
                        dir.x = -rot;
                        input.shoot = shoot;
                    })
                    .Run();

        }
    }
}