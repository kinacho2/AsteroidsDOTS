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

using AudioType = Asteroids.Data.AudioType;
using Asteroids.Audio;

namespace Asteroids.ECS.Systems
{
    public class Audio_System : SystemBase
    {
        //AudioType[] Sounds;
        private NativeArray<int> _consumers;
        private Entity AudioListener;
        private bool _initialized = false;
        private SoundManager SoundManager;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            var audioDB = Configs.AudioDB;
            if (!audioDB) return;
            SoundManager = Configs.SoundManager;

            _consumers = new NativeArray<int>(audioDB.Sounds.Length + 1, Allocator.Persistent);

            var defaultWorld = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            settings.DebugConversionName = "AudioSource";


            _consumers[((int)AudioType.PlayerStartMove)] = Events_System.OnPlayerStartMove.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerStopMove)] = Events_System.OnPlayerStopMove.Subscribe(Configs.EVENTS_QUEUE_COUNT);

            _consumers[((int)AudioType.PlayerShoot)] = Events_System.OnEntityShoot.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerCollision)] = Events_System.OnPlayerCollision.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            //_consumers[((int)AudioType.PlayerDamage)] = Events_System.OnPlayerDamage.Subscribe(Configs.EVENTS_QUEUE_COUNT);
            _consumers[((int)AudioType.PlayerDestroyed)] = Events_System.OnEntityDestroyed.Subscribe(Configs.EVENTS_QUEUE_COUNT);

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

            _initialized = true;
        }

        protected override void OnUpdate()
        {
            if (!_initialized) return;
            CheckEvent(AudioType.PlayerShoot, ref Events_System.OnEntityShoot);
            CheckEvent(AudioType.PlayerCollision, ref Events_System.OnPlayerCollision);
            CheckEvent(AudioType.PlayerDestroyed, ref Events_System.OnEntityDestroyed);
            CheckEvent(AudioType.MisileHit, ref Events_System.OnMisileHit);
            CheckEvent(AudioType.LoseShield, ref Events_System.OnPlayerLoseShield);

            if (GetEvent(_consumers[(int)AudioType.AsteroidCollisionBig], ref Events_System.OnAsteroidsCollision, out var asteroid))
            {
                switch (asteroid.type)
                {
                    case AsteroidType.Bigger:
                        PlaySound(AudioType.AsteroidCollisionBig);
                        break;
                    case AsteroidType.Medium:
                        PlaySound(AudioType.AsteroidCollisionMedium);
                        break;
                    case AsteroidType.Small:
                        PlaySound(AudioType.AsteroidCollisionSmall);
                        break;
                    case AsteroidType.Tiny:
                        PlaySound(AudioType.MisileHit);
                        break;
                }
            }

            if (GetEvent(_consumers[(int)AudioType.AsteroidDestroyedBig], ref Events_System.OnAsteroidDestroyed, out var asteroidDestroyed))
            {
                switch (asteroidDestroyed.type)
                {
                    case AsteroidType.Bigger:
                        PlaySound(AudioType.AsteroidDestroyedBig);
                        break;
                    case AsteroidType.Medium:
                        PlaySound(AudioType.AsteroidDestroyedMedium);
                        break;
                    case AsteroidType.Small:
                        PlaySound(AudioType.AsteroidDestroyedSmall);
                        break;
                }
            }

            if (GetEvent(_consumers[(int)AudioType.PickShield], ref Events_System.OnPickPower, out var power))
            {
                switch (power.type)
                {
                    case PowerType.Shield:
                        PlaySound(AudioType.PickShield);
                        break;
                    case PowerType.Weapon:
                        PlaySound(AudioType.PickWeapon);
                        break;
                    case PowerType.Bomb:
                        PlaySound(AudioType.PickBomb);
                        break;
                    case PowerType.Health:
                        PlaySound(AudioType.PickHealth);
                        break;
                }
            }

            CheckEvent(AudioType.PlayerStartMove, ref Events_System.OnPlayerStartMove, true);
            if (GetEvent(_consumers[(int)AudioType.PlayerStopMove], ref Events_System.OnPlayerStopMove, out var stopMove))
                StopSound(AudioType.PlayerStartMove);
        }

        protected void CheckEvent<T>(AudioType type, ref EventPublisher<T> publisher, bool loop = false) where T : struct
        {
            int index = (int)type;
            if (publisher.TryGetEvent(_consumers[index], out var eventData))
            {
                PlaySound(type, loop);
            }
        }

        protected bool GetEvent<T>(int consumer, ref EventPublisher<T> publisher, out T eventData) where T : struct
        {
            return publisher.TryGetEvent(consumer, out eventData);
        }

        private void PlaySound(AudioType Clip, bool loop = false)
        {
            SoundManager.PlayClip(Clip, loop);
        }
        private void StopSound(AudioType Clip)
        {
            SoundManager.StopLoopeableClip(Clip);
        }

        protected override void OnDestroy()
        {
            _consumers.Dispose();
            base.OnDestroy();
        }
    }
}