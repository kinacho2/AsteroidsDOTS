using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.ECS.Systems;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Asteroids.ECS.Events;
using Random = Unity.Mathematics.Random;
public class BombSpawn_System : SystemBase
{
    protected Entity entityPrefab;
    Random Random;
    private EventConsumer _eventConsumer;
    protected override void OnCreate()
    {
        base.OnCreate();
        Random = new Random(0x6A622EB2u);
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

        _eventConsumer = Events_System.OnPickPower.Subscribe(Configs.EVENTS_QUEUE_COUNT);
    }

    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        if (Events_System.OnPickPower.TryGetEvent(_eventConsumer, out var power))        
        {
            if(power.type == PowerType.Bomb)
                InstantiateBomb(entityPrefab, power.position, power.player, ref cmdBuffer);
        }

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }

    private void InstantiateBomb(Entity entityPrefab, float3 position, Entity player, ref EntityCommandBuffer cmdBuffer)
    {
        var radius = 0.1f;

        var entity = cmdBuffer.Instantiate(entityPrefab);
        cmdBuffer.AddComponent(entity, new BombComponent { owner = player, radius = radius, expansionSpeed = 3, lifeTime = 3, ID = Random.NextUInt() });
        cmdBuffer.AddComponent(entity, new Translation { Value = position });
        cmdBuffer.AddComponent(entity, new Scale { Value = radius });
    }

}
