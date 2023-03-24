using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Asteroids/Game Data", fileName = "GameData.asset", order = 0)]

public class GameDataSO : ScriptableObject
{
    [field: SerializeField]
    public int EnemyCount { get; private set; }

    [field: SerializeField]
    public int AsteroidCount { get; private set; }
}
