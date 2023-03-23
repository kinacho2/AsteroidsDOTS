using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Audio Data", fileName = "AudiioData.asset", order = 0)]

    public class AudioDataSO : ScriptableObject
    {
        [field: SerializeField]
        public AudioSource AudioSourcePrefab { get; private set; }

        [field: SerializeField]
        public AudioData[] Sounds { get; private set; }

        void OnValidate()
        {
            for (int i = 0; i < Sounds.Length; i++)
            {
                Sounds[i].type = (AudioType)i;
            }
        }

    }

    [System.Serializable]
    public struct AudioData
    {
        public AudioType type;
        public AudioClip clip;
    }

    public enum AudioType : int
    {
        None = -1,
        PlayerStartMove = 0,
        PlayerShoot = 1,
        PlayerCollision = 2,
        PlayerDamage = 3,
        PlayerDestroyed = 4,
        AsteroidCollisionBig = 5,
        AsteroidCollisionMedium = 6,
        AsteroidCollisionSmall = 7,
        AsteroidDestroyedBig = 8,
        AsteroidDestroyedMedium = 9,
        AsteroidDestroyedSmall = 10,
        MisileHit = 11,
        PickShield = 12,
        PickWeapon = 13,
        PickBomb = 14,
        PickHealth = 15,
        LoseShield = 16,
        PlayerStopMove = 17,
    }
}