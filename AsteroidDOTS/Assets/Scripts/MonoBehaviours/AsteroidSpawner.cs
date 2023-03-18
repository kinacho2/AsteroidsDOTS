using Asteroids.ECS.Components;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.U2D.Entities.Physics;
using UnityEngine;

namespace Asteroids.Core.Spawners
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] GameObject Prefab;

        protected static Entity entityPrefab;
        protected static World defaultWorld;
        protected static EntityManager entityManager;
        // Start is called before the first frame update
        void Start()
        {
            AMeshTools.InitializeLineShape(Prefab);
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);
            InstantiateAsteroid(new float3(2.45f, -1.45f, 5.93f), new float2(0, 0), Mathf.PI / 2);
        }

        public static void InstantiateAsteroid(float3 position, float2 velocity, float angular)
        {
            var entity = entityManager.Instantiate(entityPrefab);
            entityManager.AddComponent<LimitCheckComponent>(entity);
            entityManager.AddComponent<AsteroidComponent>(entity);
            entityManager.SetComponentData(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });

        }

    }
}
