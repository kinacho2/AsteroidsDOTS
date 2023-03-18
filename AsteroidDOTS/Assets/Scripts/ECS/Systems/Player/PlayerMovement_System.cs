using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;

namespace Asteroids.ECS.Systems
{
    public class PlayerMovement_System : SystemBase
    {

        protected override void OnUpdate()
        {

            float deltaTime = Time.DeltaTime;
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref PlayerStatsComponent move,
                    ref Translation translation,
                    ref Rotation rotation,
                    ref PhysicsVelocity physics,
                    in PlayerDataComponent stats,
                    in PlayerInputComponent input) =>
                {
                    var dspeed = input.direction.y * stats.acceleration * deltaTime;
                    if (move.stunnedTimer > 0)
                        dspeed = 0;
                    else
                        physics.Angular = input.direction.x * math.radians(stats.rotationSpeedDeg);

                    //compute velocity
                    var velocity = physics.Linear;
                    float2 fordward = math.mul(rotation.Value, math.down()).ToFloat2();
                    velocity += fordward * dspeed;

                    //clamp velocity
                    var len = math.length(velocity);
                    if (len > 0)
                    {
                        velocity = velocity / len * math.clamp(len, 0, stats.maxSpeed);
                        physics.Linear = velocity;
                    }
                })
                .Run();
        }
    }
}