using Asteroids.ECS.Components;
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
            bool charging = Input.GetKey(KeyCode.E);

            //TODO(Need to change this code with SingletonEntity)
            Entities
                .WithAll<ShipInputComponent, ShipStatsComponent, PlayerComponent>()
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref ShipInputComponent input,
                    ref ShipStatsComponent stats,
                    ref HyperspaceTravelComponent travel
                    ) =>
                {
                    ref var dir = ref input.direction;

                    if (dir.y != tr)
                    {
                        if (math.abs(tr) > math.abs(dir.y))
                            Events_System.OnPlayerStartMove.PostEvent(new Events.PlayerMove());
                        else if (math.abs(tr) < math.abs(dir.y))
                            Events_System.OnPlayerStopMove.PostEvent(new Events.PlayerMove());

                    }

                    dir.y = tr;
                    dir.x = -rot;
                    input.shoot = shoot;

                    travel.chargingPressed = charging;
                })
                .Run();
        }
    }
}