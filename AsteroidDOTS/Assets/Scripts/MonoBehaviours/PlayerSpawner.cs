using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Entities;
using UnityEngine;

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

        InitiializeLineShape();

        var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerPrefab, settings);

        InstantiatePlayerEntity();
    }

    private void InitiializeLineShape()
    {
        var polygonCollider = PlayerPrefab.GetComponent<PolygonCollider2D>();
        var meshFilter = PlayerPrefab.GetComponentInChildren<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        AMeshTools.CreateMeshWithMassCenter(polygonCollider.points, meshFilter.sharedMesh);

    }

    private void InstantiatePlayerEntity()
    {
        var entity = entityManager.Instantiate(entityPrefab);
        entityManager.AddComponent<LimitCheckComponent>(entity);
        entityManager.AddComponentData(entity, new PlayerStatsComponent
        {
            stunnedTimer = 0,
            health = PlayerData.health,

        });
        entityManager.AddComponentData(entity, new PlayerDataComponent
        {
            acceleration = PlayerData.acceleration,
            maxSpeed = PlayerData.maxSpeed,
            restitution = PlayerData.restitution,
            rotationSpeedDeg = PlayerData.rotationSpeedDeg,
            stunnedTime = PlayerData.stunnedTime,
        });

    }

}
