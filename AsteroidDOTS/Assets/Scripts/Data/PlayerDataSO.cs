using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Player Data", fileName = "PlayerData.asset", order = 0)]

    public class PlayerDataSO : ScriptableObject
    {
        [field: SerializeField]
        public float maxSpeed { get; protected set; }

        [field: SerializeField]
        public float acceleration { get; protected set; }

        [field: SerializeField]
        public float rotationSpeedDeg { get; protected set; }

        [field: SerializeField]
        public float restitution { get; protected set; }

        [field: SerializeField]
        public float stunnedTime { get; protected set; }

        [field: SerializeField]
        public float shootCooldown { get; protected set; }

        [field: SerializeField]
        public int health { get; protected set; }
    }
}