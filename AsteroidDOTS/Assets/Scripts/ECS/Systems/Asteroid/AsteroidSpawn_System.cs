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

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
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

            InstantiateFirstAsteroids(asteroidDB.InitialCount);
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
                    in AsteroidComponent asteroid,
                    in HealthComponent health
                    ) =>
                {
                    if (health.health <= 0)
                    {
                        cmdBuffer.DestroyEntity(entity);
                        Events_System.OnAsteroidDestroyed.PostEvent(new Events.AsteroidDestroyed { position = tr.Value, type = asteroid.type, size = asteroid.size });
                        int type = ((int)asteroid.type);
                        if (asteroid.type < AsteroidType.Tiny)
                        {
                            InstantiateAsteroid(type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 2f/3f), asteroid, ref cmdBuffer);
                            InstantiateAsteroid(type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 4f/3f), asteroid, ref cmdBuffer);
                            InstantiateAsteroid(type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 2), asteroid, ref cmdBuffer);
                            //InstantiateAsteroid(asteroid.type + 1, math.length(vel.Linear), tr.Value, AGeometry.RotateZ(asteroid.explodeDirection, math.PI * 1.75f), asteroid.size, ref cmdBuffer);
                        }
                        if(asteroid.type < AsteroidType.Small)
                        {
                            //power ups
                            //var rand = Random.NextUInt(0, 10);
                            //if (rand < 9)
                            {
                                //PowerSpawn_System.SpawnQueue.Enqueue(tr.Value);
                            }
                        }
                    }
                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private void InstantiateAsteroid(int type, float currentSpeed, float3 position, float2 direction, AsteroidComponent parentAsteroid, ref EntityCommandBuffer cmdBuffer)
        {
            int startIdx = GetFirstEntityPrefabIndex(type);

            var randIdx = Random.NextUInt() % data[type].shapes.Length + startIdx;

            InstantiateAsteroid(
                entityPrefabs[randIdx],
                GetPerpendicularPosition(parentAsteroid.size * 0.5f, position.ToFloat2(), direction),
                GetPerpendicularVelocity(currentSpeed, data[type].maxSpeed, 25, direction),
                math.radians(Random.NextFloat(0, math.PI)),
                data[type], parentAsteroid,
                ref cmdBuffer
                );
        }

        private void InstantiateAsteroid(Entity entityPrefab, float2 position, float2 velocity, float angular, AsteroidData data, AsteroidComponent parentAsteroid, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(entityPrefab);
            cmdBuffer.AddComponent<LimitCheckComponent>(entity);
            cmdBuffer.AddComponent(entity, new AsteroidComponent
            {
                type = data.type,
                size = data.size,
                maxSpeed = data.maxSpeed,
                lastBombID = parentAsteroid.lastBombID,
            });
            cmdBuffer.AddComponent(entity, new HealthComponent
            {
                health = data.health,
            });
            cmdBuffer.AddComponent(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });
            cmdBuffer.AddComponent(entity, new Translation { Value = position.ToFloat3() });
        }

        private void InstantiateFirstAsteroids(int count)
        {
            var asteroidData = data[0];
            var type = asteroidData.type;

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
                    type = type, 
                    size = asteroidData.size,
                    maxSpeed = asteroidData.maxSpeed,
                });
                EntityManager.AddComponentData(entity, new HealthComponent
                {
                    health = asteroidData.health,
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