using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Entities;
using UnityEngine;
using System.Linq;
using Asteroids.Setup;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] PlayerDataSO PlayerData;

    private Entity entityPrefab;
    private World defaultWorld;
    private EntityManager entityManager;
    void Start()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        InitializeLineShape();

        var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerPrefab, settings);

        InstantiatePlayerEntity();
    }

    private void InitializeLineShape()
    {
        var polygonCollider = PlayerPrefab.GetComponent<PolygonCollider2D>();
        var meshFilter = PlayerPrefab.GetComponentInChildren<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        AMeshTools.CreateMeshWithMassCenter(polygonCollider.points, PlayerPrefab.transform.localScale, meshFilter.sharedMesh);

        var shieldMeshFilter = PlayerPrefab.GetComponentsInChildren<MeshFilter>().Where((x) => x.tag == "Shield").FirstOrDefault();
        if (shieldMeshFilter)
            AMeshTools.CreateCircleMesh(shieldMeshFilter, 0.5f, 20);
    }

    private void InstantiatePlayerEntity()
    {
        var entity = entityManager.Instantiate(entityPrefab);
        entityManager.AddComponent<LimitCheckComponent>(entity);
        entityManager.AddComponentData(entity, new PlayerStatsComponent
        {
            stunnedTimer = 0,
            health = PlayerData.health,
            shieldHealth = PlayerData.shieldHealth,
        });
        entityManager.AddComponentData(entity, new PlayerDataComponent
        {
            maxHealth = PlayerData.health,
            shieldHealth = PlayerData.shieldHealth,
            acceleration = PlayerData.acceleration,
            maxSpeed = PlayerData.maxSpeed,
            restitution = PlayerData.restitution,
            rotationSpeedDeg = PlayerData.rotationSpeedDeg,
            stunnedTime = PlayerData.stunnedTime,
            shootCooldown = PlayerData.shootCooldown,
        });
        var weaponData = Configs.WeaponDB.Get(0);
        entityManager.AddComponentData(entity, new PlayerWeaponComponent
        {
            misileAmount = weaponData.misileAmount,
            misileSpeed = weaponData.misileSpeed,
            misileLifeTime = weaponData.misileLifeTime,
            range = weaponData.range,
            type = 0,
        });
    }

}
