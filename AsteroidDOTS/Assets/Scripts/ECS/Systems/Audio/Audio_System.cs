using Asteroids.Audio;
using Asteroids.Data;
using Asteroids.ECS.Events;
using Asteroids.Setup;
using Unity.Collections;
using Unity.Entities;
using AudioType = Asteroids.Data.AudioType;

namespace Asteroids.ECS.Systems
{
    public class Audio_System : SystemBase
    {
        private NativeArray<EventConsumer> _consumers;

        private bool _initialized = false;
        private SoundManager SoundManager;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;

            var audioDB = Configs.AudioDB;
            if (!audioDB) return;
            SoundManager = Configs.SoundManager;

            _consumers = new NativeArray<EventConsumer>(audioDB.Sounds.Length + 2, Allocator.Persistent);

            _consumers[((int)AudioType.PlayerStartMove)] = Events_System.OnPlayerStartMove.Subscribe();
            _consumers[((int)AudioType.PlayerStopMove)] = Events_System.OnPlayerStopMove.Subscribe();

            _consumers[((int)AudioType.PlayerShoot)] = Events_System.OnEntityShoot.Subscribe();
            _consumers[((int)AudioType.PlayerCollision)] = Events_System.OnPlayerCollision.Subscribe();
            _consumers[((int)AudioType.PlayerDestroyed)] = Events_System.OnEntityDestroyed.Subscribe();

            _consumers[((int)AudioType.AsteroidCollisionBig)] =
            _consumers[((int)AudioType.AsteroidCollisionMedium)] =
            _consumers[((int)AudioType.AsteroidCollisionSmall)] = Events_System.OnAsteroidsCollision.Subscribe();

            _consumers[((int)AudioType.AsteroidDestroyedBig)] =
            _consumers[((int)AudioType.AsteroidDestroyedMedium)] =
            _consumers[((int)AudioType.AsteroidDestroyedSmall)] = Events_System.OnAsteroidDestroyed.Subscribe();

            _consumers[((int)AudioType.MissileHit)] = Events_System.OnMissileHit.Subscribe();

            _consumers[((int)AudioType.PickShield)] =
            _consumers[((int)AudioType.PickWeapon)] =
            _consumers[((int)AudioType.PickBomb)] =
            _consumers[((int)AudioType.PickHealth)] = Events_System.OnPickPower.Subscribe();

            _consumers[((int)AudioType.LoseShield)] = Events_System.OnPlayerLoseShield.Subscribe();

            _consumers[((int)AudioType.WarpJump)] = Events_System.OnHyperspaceTravelStart.Subscribe();
            _consumers[((int)AudioType.WarpJumpStop)] = Events_System.OnHyperspaceTravelStop.Subscribe();

            _initialized = true;
        }

        protected override void OnUpdate()
        {
            if (!_initialized) return;
            CheckEvent(AudioType.PlayerShoot, ref Events_System.OnEntityShoot);
            CheckEvent(AudioType.PlayerCollision, ref Events_System.OnPlayerCollision);
            CheckEvent(AudioType.MissileHit, ref Events_System.OnMissileHit);
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
                        PlaySound(AudioType.MissileHit);
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

            CheckEvent(AudioType.WarpJump, ref Events_System.OnHyperspaceTravelStart, true);
            if (GetEvent(_consumers[(int)AudioType.WarpJumpStop], ref Events_System.OnHyperspaceTravelStop, out var stopJump))
                StopSound(AudioType.WarpJump);

            if (GetEvent(_consumers[(int)AudioType.PlayerDestroyed], ref Events_System.OnEntityDestroyed, out var destoyEvent))
            {
                PlaySound(AudioType.PlayerDestroyed);
                if (destoyEvent.entityType == EntityType.Player)
                {
                    StopSound(AudioType.WarpJump);
                    StopSound(AudioType.PlayerStartMove);
                }
            }

        }

        protected void CheckEvent<T>(AudioType type, ref EventPublisher<T> publisher, bool loop = false) where T : struct
        {
            int index = (int)type;
            if (publisher.TryGetEvent(_consumers[index], out var eventData))
            {
                PlaySound(type, loop);
            }
        }

        protected bool GetEvent<T>(EventConsumer consumer, ref EventPublisher<T> publisher, out T eventData) where T : struct
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