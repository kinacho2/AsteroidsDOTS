using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Entities;
using UnityEngine;
using System.Linq;
using Asteroids.Setup;
using Unity.Transforms;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] PlayerDataSO PlayerData;

    private Entity shipPrefab;
    private World defaultWorld;
    private EntityManager entityManager;
    void Start()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        InitializeLineShape();

        var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        shipPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerPrefab, settings);

        InstantiatePlayerEntity();
        InstantiateEnemyEntity();
    }

    private void InitializeLineShape()
    {
        //var polygonCollider = PlayerPrefab.GetComponent<PolygonCollider2D>();
        var meshFilter = PlayerPrefab.GetComponentInChildren<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        AMeshTools.CreateMeshWithMassCenter(PlayerData.shape, PlayerPrefab.transform.localScale, meshFilter.sharedMesh);


        var shieldMeshFilter = PlayerPrefab.GetComponentsInChildren<MeshFilter>().Where((x) => x.tag == "Shield").FirstOrDefault();
        if (shieldMeshFilter)
            AMeshTools.CreateCircleMesh(shieldMeshFilter, 0.5f, 20);
    }

    private void InstantiatePlayerEntity()
    {
        var entity = InstantiateShipEntity();
        entityManager.AddComponent<PlayerComponent>(entity);
        var weaponData = Configs.PlayerData.WeaponsDB.Get(0);
        entityManager.AddComponentData(entity, new WeaponComponent
        {
            misileAmount = weaponData.misileAmount,
            misileSpeed = weaponData.misileSpeed,
            misileLifeTime = weaponData.misileLifeTime,
            range = weaponData.range,
            type = 0,
        });
    }

    private void InstantiateEnemyEntity()
    {
        var entity = InstantiateShipEntity();
        entityManager.AddComponent<EnemyComponent>(entity);
        var pos = Configs.GetRandomPositionOutOfScreen();
        entityManager.SetComponentData(entity, new Translation { Value = pos.ToFloat3() });
    }

    protected Entity InstantiateShipEntity()
    {
        var entity = entityManager.Instantiate(shipPrefab);
        entityManager.AddComponent<LimitCheckComponent>(entity);
        entityManager.AddComponent<ShipInputComponent>(entity);
        entityManager.AddComponentData(entity, new ShipStatsComponent
        {
            stunnedTimer = 0,
            shieldHealth = 0,
        });
        entityManager.AddComponentData(entity, new HealthComponent
        {
            health = PlayerData.health
        });
        entityManager.AddComponentData(entity, new ShipDataComponent
        {
            maxHealth = PlayerData.health,
            shieldHealth = PlayerData.shieldHealth,
            acceleration = PlayerData.acceleration,
            maxSpeed = PlayerData.maxSpeed,
            restitution = PlayerData.restitution,
            rotationSpeedDeg = PlayerData.rotationSpeedDeg,
            stunnedTime = PlayerData.stunnedTime,
            invTime = PlayerData.invTime,
            shootCooldown = PlayerData.shootCooldown,
        });
        return entity;
    }

}
