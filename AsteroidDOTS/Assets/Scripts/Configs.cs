using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids.Setup
{
    public class Configs
    {
        public static float2 CameraLimits { get; protected set; }

        public static void InitializeConfigs(Camera camera)
        {
            var worldPoint = camera.ViewportToWorldPoint(Vector2.one);
            CameraLimits = new float2(worldPoint.y * camera.pixelWidth / camera.pixelHeight, worldPoint.y);
        }
    }
}
