using Asteroids.ECS.Components;
using Asteroids.Setup;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids.ECS.Systems
{
    public class LimitsCheck_System : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAny<LimitCheckComponent>()
                   .WithoutBurst()
                   .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation) =>
                   {

                       //check camera limits
                       ref var tr = ref translation.Value;
                       var cameraLimits = Configs.CameraLimits;
                       if (math.abs(tr.x) > cameraLimits.x)
                           tr.x = -math.sign(tr.x) * cameraLimits.x;
                       if (math.abs(tr.y) > cameraLimits.y)
                           tr.y = -math.sign(tr.y) * cameraLimits.y;
                   })
                   .Run();

        }
    }
}