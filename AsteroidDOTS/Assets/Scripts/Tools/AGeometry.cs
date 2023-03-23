using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids.Tools
{
    public static class AGeometry
    {
        public static float2 RotateZ(float2 dir, float rads)
        {
            float x = dir.x;
            float y = dir.y;
            dir.x = x * Mathf.Cos(rads) - y * Mathf.Sin(rads);
            dir.y = x * Mathf.Sin(rads) + y * Mathf.Cos(rads);
            return dir;
        }

        public static Vector2 RotateZ(Vector2 dir, float rads)
        {
            float x = dir.x;
            float y = dir.y;
            dir.x = x * Mathf.Cos(rads) - y * Mathf.Sin(rads);
            dir.y = x * Mathf.Sin(rads) + y * Mathf.Cos(rads);
            return dir;
        }

        public static float2 GetPerpendicular(float2 dir)
        {
            float aux = dir.x;
            dir.x = dir.y;
            dir.y = aux;
            return dir;
        }

        public static Vector2 ToVector2(this float2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static float2 ToFloat2(this float3 vector)
        {
            return new float2(vector.x, vector.y);
        }

        public static float3 ToFloat3(this float2 vector)
        {
            return new float3(vector.x, vector.y, 0);
        }

        public static Vector3 ToVector3(this float3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static Vector3 ToVector3(this float2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }
    }

}