using Asteroids.Setup;
using Asteroids.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
//using Unity.Tiny.Audio;
//using AudioSource = Unity.Tiny.Audio.AudioSource;
//using AudioListener = Unity.Tiny.Audio.AudioListener;

namespace Asteroids.ECS.Systems
{
    public class Audio_System : SystemBase
    {
        AudioData[] Sounds;
        private NativeArray<int> _consumers;
        private Entity AudioListener;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            var audioDB = Configs.AudioDB;
            Sounds = audioDB.Sounds;

            _consumers = new NativeArray<int>(Sounds.Length, Allocator.Persistent);

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            settings.DebugConversionName = "AudioSource";


            //_consumers[((int)AudioType.PlayerMove)] = Events_System.OnPlayerShoot.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerShoot)] = Events_System.OnPlayerShoot.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerCollision)] = Events_System.OnPlayerCollision.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            //_consumers[((int)AudioType.PlayerDamage)] = Events_System.OnPlayerDamage.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerDestroyed)] = Events_System.OnPlayerDestroyed.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.AsteroidCollisionBig)] = 
            _consumers[((int)AudioType.AsteroidCollisionMedium)] =
            _consumers[((int)AudioType.AsteroidCollisionSmall)] = Events_System.OnAsteroidsCollision.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.AsteroidDestroyedBig)] =
            _consumers[((int)AudioType.AsteroidDestroyedMedium)] =
            _consumers[((int)AudioType.AsteroidDestroyedSmall)] = Events_System.OnAsteroidDestroyed.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.MisileHit)] = Events_System.OnMisileHit.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.PickShield)] =
            _consumers[((int)AudioType.PickWeapon)] =
            _consumers[((int)AudioType.PickBomb)] =
            _consumers[((int)AudioType.PickHealth)] = Events_System.OnPickPower.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.LoseShield)] = Events_System.OnPlayerLoseShield.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            /*
             * 
             *  PlayerMove = 0,
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
            /**/
        }

        protected override void OnUpdate()
        {
            CheckEvent(AudioType.PlayerShoot, ref Events_System.OnPlayerShoot);
            CheckEvent(AudioType.PlayerCollision, ref Events_System.OnPlayerCollision);
            CheckEvent(AudioType.PlayerDestroyed, ref Events_System.OnPlayerDestroyed);
            CheckEvent(AudioType.MisileHit, ref Events_System.OnMisileHit);
            CheckEvent(AudioType.LoseShield, ref Events_System.OnPlayerLoseShield);

            if (GetEvent(_consumers[(int)AudioType.AsteroidCollisionBig], ref Events_System.OnAsteroidsCollision, out var asteroid))
            {
                switch (asteroid.type)
                {
                    case (int)AsteroidType.Bigger:
                        PlaySound(Sounds[(int)AudioType.AsteroidCollisionBig].clip);
                        break;
                    case (int)AsteroidType.Medium:
                        PlaySound(Sounds[(int)AudioType.AsteroidCollisionMedium].clip);
                        break;
                    case (int)AsteroidType.Small:
                        PlaySound(Sounds[(int)AudioType.AsteroidCollisionSmall].clip);
                        break;
                    case (int)AsteroidType.Tiny:
                        PlaySound(Sounds[(int)AudioType.MisileHit].clip);
                        break;
                }
            }

            if (GetEvent(_consumers[(int)AudioType.AsteroidDestroyedBig], ref Events_System.OnAsteroidDestroyed, out var asteroidDestroyed))
            {
                switch (asteroidDestroyed.type)
                {
                    case (int)AsteroidType.Bigger:
                        PlaySound(Sounds[(int)AudioType.AsteroidDestroyedBig].clip);
                        break;
                    case (int)AsteroidType.Medium:
                        PlaySound(Sounds[(int)AudioType.AsteroidDestroyedMedium].clip);
                        break;
                    case (int)AsteroidType.Small:
                        PlaySound(Sounds[(int)AudioType.AsteroidDestroyedSmall].clip);
                        break;
                }
            }

            if (GetEvent(_consumers[(int)AudioType.PickShield], ref Events_System.OnPickPower, out var power))
            {
                switch (power.type)
                {
                    case (int)PowerType.Shield:
                        PlaySound(Sounds[(int)AudioType.PickShield].clip);
                        break;
                    case (int)PowerType.Weapon:
                        PlaySound(Sounds[(int)AudioType.PickWeapon].clip);
                        break;
                    case (int)PowerType.Bomb:
                        PlaySound(Sounds[(int)AudioType.PickBomb].clip);
                        break;
                    case (int)PowerType.Health:
                        PlaySound(Sounds[(int)AudioType.PickHealth].clip);
                        break;
                }
            }

        }

        protected void CheckEvent<T>(AudioType type, ref EventPublisher<T> publisher) where T : struct
        {
            int index = (int)type;
            if (publisher.TryGetEvent(_consumers[index], out var eventData))
            {
                PlaySound(Sounds[index].clip);
            }
        }

        protected bool GetEvent<T>(int consumer, ref EventPublisher<T> publisher, out T eventData) where T : struct
        {
            return publisher.TryGetEvent(consumer, out eventData);
        }

        private void PlaySound(AudioClip Clip, bool loop = false)
        {
            PlayClip_System.PlayClip(Clip);
        }

        protected override void OnDestroy()
        {
            _consumers.Dispose();
            base.OnDestroy();
        }
    }
}