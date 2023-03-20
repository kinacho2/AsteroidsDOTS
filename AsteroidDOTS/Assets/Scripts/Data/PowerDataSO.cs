using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Power Data", fileName = "PowerData.asset", order = 0)]

    public class PowerDataSO : ScriptableObject
    {
        [field: SerializeField]
        public GameObject Prefab { get; protected set; }
        [field: SerializeField] 
        public float CatchRadius { get; protected set; }

        [field: SerializeField]
        public Material PowerMaterial { get; protected set; }
        [field: SerializeField]
        public Material CircleMaterial { get; protected set; }

        [SerializeField] PowerData[] Powers;

        public PowerData Get(int idx)
        {
            return Powers[idx];
        }

        public int Count => Powers.Length;
    }

    [System.Serializable]
    public struct PowerData
    {
        public PowerType Power;
        public Vector2[] shape;
        public Color shapeColor;
    }

    public enum PowerType
    {
        Shield = 0,
        Weapon = 1,
        Bomb = 2,
        Health = 3,
    }
}