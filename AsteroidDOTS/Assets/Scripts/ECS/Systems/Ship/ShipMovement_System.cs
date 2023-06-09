using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;

namespace Asteroids.ECS.Systems
{
    public class ShipMovement_System : SystemBase
    {
        protected override void OnUpdate()
        {

            float deltaTime = Time.DeltaTime;
            Entities
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref ShipStatsComponent stats,
                    ref PhysicsVelocity physics,
                    in Rotation rotation,
                    in ShipDataComponent data,
                    in ShipInputComponent input) =>
                {
                    var dspeed = input.direction.y * data.acceleration * deltaTime;
                    if (stats.stunnedTimer > 0)
                        dspeed = 0;
                    else
                        physics.Angular = input.direction.x * math.radians(data.rotationSpeedDeg);

                    //compute velocity
                    var velocity = physics.Linear;
                    var fordward = math.mul(rotation.Value, math.down()).ToFloat2();
                    velocity += fordward * dspeed;

                    //clamp velocity
                    var len = math.length(velocity);
                    if (len > 0)
                    {
                        velocity = velocity / len * math.clamp(len, 0, data.maxSpeed);
                        physics.Linear = velocity;
                    }
                })
                .Run();
        }
    }
}