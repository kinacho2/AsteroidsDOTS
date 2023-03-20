using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Asteroids.ECS.Systems
{
    public class AsteroidSpawn_System : SystemBase
    {
        protected const float PI2 = math.PI * 2;
        protected Entity[] entityPrefabs;
        protected AsteroidData[] data;
        protected World defaultWorld;
        //protected EntityManager entityManager;

        Random Random;
        protected float2 CameraLimits;
        protected float distance;
        protected override void OnCreate()
        {
            base.OnCreate();
            Random = new Random(0x6E624EB2u);
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;
            CameraLimits = Configs.CameraLimits;
            distance = math.length(CameraLimits + new float2(1, 1));
            var asteroidDB = Configs.AsteroidDB;
            entityPrefabs = new Entity[asteroidDB.ShapesCount()];
            data = new AsteroidData[asteroidDB.DataCount];

            defaultWorld = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);

            int k = 0;
            for (int i = 0; i < asteroidDB.DataCount; i++)
            {
                var asteroidData = Configs.AsteroidDB.Get((AsteroidType)i);

                data[i] = asteroidData;

                for (int j = 0; j < asteroidData.shapes.Length; j++)
                {
                    SetParameters(Configs.AsteroidDB.Prefab, j, ref asteroidData);
                    //var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
                    entityPrefabs[k] = GameObjectConversionUtility.ConvertGameObjectHierarchy(Configs.AsteroidDB.Prefab, settings);
                    k++;
                }
            }

            InstantiateFirstAsteroids(6);
        }

        protected void SetParameters(GameObject prefab, int shapeIndex, ref AsteroidData asteroidData)
        {
            AMeshTools.InitializePolygonShape(Configs.AsteroidDB.Prefab, asteroidData.shapes[shapeIndex].points);
            prefab.GetComponent<Rigidbody2D>().mass = asteroidData.mass;

        }

        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            Entities
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    in PhysicsVelocity vel,
                    in Translation tr,
                    in AsteroidComponent asteroid
                    ) =>
                {
                    if (asteroid.health <= 0)
                    {
                        cmdBuffer.DestroyEntity(entity);
                        if (asteroid.type < (int)AsteroidType.Tiny)
                        {
                            InstantiateAsteroid(asteroid.type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 2f/3f), asteroid.size, ref cmdBuffer);
                            InstantiateAsteroid(asteroid.type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 4f/3f), asteroid.size, ref cmdBuffer);
                            InstantiateAsteroid(asteroid.type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 2), asteroid.size, ref cmdBuffer);
                            //InstantiateAsteroid(asteroid.type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 1.75f), asteroid.size, ref cmdBuffer);
                        }
                        //Spawn asteroids
                    }
                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private void InstantiateAsteroid(int type, float currentSpeed, float3 position, float2 direction, float size, ref EntityCommandBuffer cmdBuffer)
        {
            int startIdx = GetFirstEntityPrefabIndex(type);

            var randIdx = Random.NextUInt() % data[type].shapes.Length + startIdx;

            InstantiateAsteroid(
                entityPrefabs[randIdx],
                GetPerpendicularPosition(size * 0.5f, position.ToFloat2(), direction),
                GetPerpendicularVelocity(currentSpeed, data[type].maxSpeed, 25, direction),
                math.radians(Random.NextFloat(0, math.PI)),
                data[type],
                ref cmdBuffer
                );
        }

        private void InstantiateAsteroid(Entity entityPrefab, float2 position, float2 velocity, float angular, AsteroidData data, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(entityPrefab);
            cmdBuffer.AddComponent<LimitCheckComponent>(entity);
            cmdBuffer.AddComponent(entity, new AsteroidComponent
            {
                health = data.health,
                type = (int)data.type,
                size = data.size,
                maxSpeed = data.maxSpeed,
            });
            cmdBuffer.AddComponent(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });
            cmdBuffer.AddComponent(entity, new Translation { Value = position.ToFloat3() });
        }

        private void InstantiateFirstAsteroids(int count)
        {
            var asteroidData = data[0];
            var type = (int)asteroidData.type;

            for (int i = 0; i < count; i++)
            {

                var entityPrefab = entityPrefabs[Random.NextInt(0, asteroidData.shapes.Length) % asteroidData.shapes.Length];
                var position = GetRandomPositionOutOfScreen() * 0.5f;
                var velocity = GetRandomVelocity(asteroidData.maxSpeed);
                var angular = math.radians(Random.NextFloat(0, math.PI));

                var entity = EntityManager.Instantiate(entityPrefab);
                EntityManager.AddComponent<LimitCheckComponent>(entity);
                EntityManager.AddComponentData(entity, new AsteroidComponent 
                {
                    health = asteroidData.health, 
                    type = type, 
                    size = asteroidData.size,
                    maxSpeed = asteroidData.maxSpeed,
                });
                EntityManager.AddComponentData(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });
                EntityManager.AddComponentData(entity, new Translation { Value = position.ToFloat3() });
            }
        }

        private int GetFirstEntityPrefabIndex(int type)
        {
            int index = 0;
            for (int i = 0; i < type; i++)
            {
                index += data[i].shapes.Length;
            }
            return index;
        }

        private float2 GetRandomPositionOutOfScreen()
        {
            var dir = AGeometry.RotateZ(math.right().ToFloat2(), Random.NextFloat(0, PI2));
            return dir * distance;
        }

        private float2 GetRandomVelocity(float maxSpeed)
        {
            var speed = Random.NextFloat(0, maxSpeed);
            var angle = Random.NextFloat(0, PI2);
            return AGeometry.RotateZ(math.right().ToFloat2(), angle) * speed;
        }

        private float2 GetPerpendicularVelocity(float minSpeed, float maxSpeed, float offsetAngle, float2 direction)
        {
            var angle = Random.NextFloat(0, math.radians(offsetAngle)); //math.radians(Random.NextUInt() % offsetAngle);
            var speed = Random.NextFloat(minSpeed, maxSpeed);
            return AGeometry.RotateZ(direction, angle + math.PI * 0.5f) * speed;
        }

        private float2 GetPerpendicularPosition(float dist, float2 position, float2 direction)
        {
            return position + AGeometry.RotateZ(direction, math.PI * 0.5f) * dist;
        }


    }
}