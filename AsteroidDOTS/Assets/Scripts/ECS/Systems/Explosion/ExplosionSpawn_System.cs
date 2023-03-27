using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.ECS.Events;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class ExplosionSpawn_System : SystemBase
    {
        protected Entity entityPrefab;
        private EventConsumer _shipConsumer;
        private EventConsumer _asteroidConsumer;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;
            var weaponsDB = Configs.EnemyDB.WeaponsDB;

            var prefab = weaponsDB.BombPrefab;

            AMeshTools.CreateCircleMesh(prefab.GetComponent<MeshFilter>(), 0.3f, 20);

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);

            _shipConsumer = Events_System.OnEntityDestroyed.Subscribe();
            _asteroidConsumer = Events_System.OnAsteroidDestroyed.Subscribe();
        }

        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            if (Events_System.OnEntityDestroyed.TryGetEvent(_shipConsumer, out var destroy))
            {
                var explosion = new ExplosionComponent { radius = 0.1f, expansionSpeed = 4, lifeTime = 0.7f };
                InstantiateExplosion(entityPrefab, destroy.position, ref cmdBuffer, explosion);

            }
            if (Events_System.OnAsteroidDestroyed.TryGetEvent(_asteroidConsumer, out var asteroid))
            {
                if (asteroid.type < AsteroidType.Tiny)
                {
                    var explosion = new ExplosionComponent { radius = 0.1f, expansionSpeed = 4, lifeTime = 0.6f * asteroid.size };
                    InstantiateExplosion(entityPrefab, asteroid.position, ref cmdBuffer, explosion);
                }
            }

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private void InstantiateExplosion(Entity entityPrefab, float3 position, ref EntityCommandBuffer cmdBuffer, ExplosionComponent explosion)
        {
            var entity = cmdBuffer.Instantiate(entityPrefab);
            cmdBuffer.AddComponent(entity, explosion);
            cmdBuffer.AddComponent(entity, new Translation { Value = position });
            cmdBuffer.AddComponent(entity, new Scale { Value = explosion.radius });
        }
    }
}