using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.ECS.Events;
using Asteroids.Setup;
using Asteroids.Tools;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Asteroids.ECS.Systems
{
    public class PowerSpawn_System : SystemBase
    {
        //public static NativeQueue<float3> SpawnQueue;
        protected Entity[] entityPrefabs;
        private EventConsumer _eventConsumer;

        Random Random;
        protected override void OnCreate()
        {
            base.OnCreate();
            //SpawnQueue = new NativeQueue<float3>(Allocator.Persistent);
            Random = new Random(0x6E622EA2u);
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;

            var powerDB = Configs.PowerDB;
            entityPrefabs = new Entity[powerDB.Count];
            var prefab = powerDB.Prefab;

            var circleCollider = prefab.GetComponent<CircleCollider2D>();
            circleCollider.radius = powerDB.CatchRadius;

            var meshFilter = prefab.GetComponentInChildren<MeshFilter>();
            var shieldMeshFilter = prefab.GetComponentsInChildren<MeshFilter>().Where((x) => x.tag == "Shield").FirstOrDefault();
            meshFilter.sharedMesh = new Mesh();

            var meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();
            var shieldMeshRenderer = prefab.GetComponentsInChildren<MeshRenderer>().Where((x) => x.tag == "Shield").FirstOrDefault();

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);

            for (int i = 0; i < powerDB.Count; i++)
            {
                var powerData = powerDB.Get(i);
                meshFilter.sharedMesh = new Mesh();
                AMeshTools.CreateMeshWithMassCenter(powerData.shape, prefab.transform.localScale, meshFilter.sharedMesh);
                AMeshTools.CreateCircleMesh(shieldMeshFilter, powerDB.CatchRadius, 20);

                meshRenderer.sharedMaterial = new Material(powerDB.PowerMaterial);
                meshRenderer.sharedMaterial.SetColor("_Color", powerData.shapeColor);

                shieldMeshRenderer.sharedMaterial = new Material(powerDB.CircleMaterial);
                shieldMeshRenderer.sharedMaterial.SetColor("_Color", Color.white);

                entityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            }

            _eventConsumer = Events_System.OnAsteroidDestroyed.Subscribe(Configs.EVENTS_QUEUE_COUNT);
        }


        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            if (Events_System.OnAsteroidDestroyed.TryGetEvent(_eventConsumer, out var asteroidEvent))
            {
                if (asteroidEvent.type < AsteroidType.Small)
                {
                    var type = Random.NextInt(0, entityPrefabs.Length) % entityPrefabs.Length;
                    //type = 0;
                    var entityPrefab = entityPrefabs[type];
                    InstantiatePower(entityPrefab, (PowerType)type, asteroidEvent.position, ref cmdBuffer);
                }
            }

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private void InstantiatePower(Entity entityPrefab, PowerType type, float3 position, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(entityPrefab);
            cmdBuffer.AddComponent(entity, new PowerComponent { type = type });
            cmdBuffer.AddComponent(entity, new Translation { Value = position });
        }


        protected override void OnDestroy()
        {
            //SpawnQueue.Dispose();
            base.OnDestroy();
        }
    }
}