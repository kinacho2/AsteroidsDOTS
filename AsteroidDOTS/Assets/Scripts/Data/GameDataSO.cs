using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Asteroids/Game Data", fileName = "GameData.asset", order = 0)]

public class GameDataSO : ScriptableObject
{
    [field: SerializeField]
    public SpawnData EnemiesSpawnData { get; private set; }

    [field: SerializeField]
    public SpawnData AsteroidsSpawnData { get; private set; }

    [field: SerializeField]
    public HyperspaceTravelData HyperspaceTravelData { get; private set; }

}

[System.Serializable]
public struct SpawnData
{
    public int initialEntityCount;
    public int entityCount;
    public float spawnSeconds;
}

[System.Serializable]
public struct HyperspaceTravelData
{
    public float timeBeforeTravel;
    public float timeAfterTravel;
    public float timeReloading;
}