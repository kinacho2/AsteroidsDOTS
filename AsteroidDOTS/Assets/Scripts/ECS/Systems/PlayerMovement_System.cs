using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

public class PlayerMovement_System : JobComponentSystem
{
    

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithBurst()
            .ForEach((Entity entity, int entityInQueryIndex, PlayerInputComponent input, ref PlayerMoveComponent move, ref Translation translation, ref Rotation rotation) =>
                {
                    var dspeed = input.direction.y * move.acceleration * deltaTime;
                    var drotation = input.direction.x * math.radians(move.rotationSpeedDeg) * deltaTime;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(drotation));

                    ref var velocity = ref move.velocity;
                    var fordward = math.mul(rotation.Value, math.down());

                    velocity += fordward * dspeed;
                    var len = math.length(velocity);
                    if (len > 0)
                    {
                        velocity = velocity / len * math.clamp(len, 0, move.maxSpeed);

                        translation.Value += velocity * deltaTime;

                        ref var value = ref translation.Value;
                        if (math.abs(value.x) > move.cameraLimits.x)
                            value.x = -value.x;
                        if (math.abs(value.y) > move.cameraLimits.y)
                            value.y = -value.y;
                    }

                    

                })
                .Run();

        return default;
    }
}
