using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
public class BombSpawn_System : SystemBase
{
    public static NativeQueue<float3> SpawnQueue;
    protected Entity entityPrefab;
    Random Random;

    protected override void OnCreate()
    {
        base.OnCreate();
        Random = new Random(0x6A622EB2u);
        SpawnQueue = new NativeQueue<float3>(Allocator.Persistent);
        Configs.OnInitializedConfig += Configs_OnInitializedConfig;
    }

    private void Configs_OnInitializedConfig()
    {
        Configs.OnInitializedConfig -= Configs_OnInitializedConfig;
        var weaponsDB = Configs.PlayerData.WeaponsDB;

        var prefab = weaponsDB.BombPrefab;

        AMeshTools.CreateCircleMesh(prefab.GetComponent<MeshFilter>(), 0.5f, 20);


        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
    }

    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        if (SpawnQueue.TryDequeue(out float3 position))
        {
            InstantiateBomb(entityPrefab, position, ref cmdBuffer);
        }

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }

    private void InstantiateBomb(Entity entityPrefab, float3 position, ref EntityCommandBuffer cmdBuffer)
    {
        var radius = 0.1f;

        var entity = cmdBuffer.Instantiate(entityPrefab);
        cmdBuffer.AddComponent(entity, new BombComponent { radius = radius, expansionSpeed = 3, lifeTime = 3, ID = Random.NextUInt() });
        cmdBuffer.AddComponent(entity, new Translation { Value = position });
        cmdBuffer.AddComponent(entity, new Scale { Value = radius });
    }

    protected override void OnDestroy()
    {
        SpawnQueue.Dispose();
        base.OnDestroy();
    }
}
