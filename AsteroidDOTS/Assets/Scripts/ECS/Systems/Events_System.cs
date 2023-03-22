using Asteroids.ECS.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class Events_System : SystemBase
    {
        private static EventPublisher<PlayerMove> _onPlayerMove;
        public static ref EventPublisher<PlayerMove> OnPlayerMove => ref _onPlayerMove;

        private static EventPublisher<PlayerShoot> _onPlayerShoot;
        public static ref EventPublisher<PlayerShoot> OnPlayerShoot => ref _onPlayerShoot;

        private static EventPublisher<PlayerCollision> _onPlayerCollision;
        public static ref EventPublisher<PlayerCollision> OnPlayerCollision => ref _onPlayerCollision;

        private static EventPublisher<PlayerDestroyed> _onPlayerDestroyed;
        public static ref EventPublisher<PlayerDestroyed> OnPlayerDestroyed => ref _onPlayerDestroyed;
        
        private static EventPublisher<PlayerLoseShield> _onPlayerLoseShield;
        public static ref EventPublisher<PlayerLoseShield> OnPlayerLoseShield => ref _onPlayerLoseShield;
        
        private static EventPublisher<AsteroidsCollision> _onAsteroidsCollision;
        public static ref EventPublisher<AsteroidsCollision> OnAsteroidsCollision => ref _onAsteroidsCollision;

        private static EventPublisher<AsteroidDestroyed> _onAsteroidDestroyed;
        public static ref EventPublisher<AsteroidDestroyed> OnAsteroidDestroyed => ref _onAsteroidDestroyed;

        private static EventPublisher<PickPower> _onPickPower;
        public static ref EventPublisher<PickPower> OnPickPower => ref _onPickPower;

        private static EventPublisher<MisileHit> _onMisileHit;
        public static ref EventPublisher<MisileHit> OnMisileHit => ref _onMisileHit;

        protected override void OnCreate()
        {
            base.OnCreate();
            _onPlayerMove = new EventPublisher<PlayerMove>(20);
            _onPlayerShoot = new EventPublisher<PlayerShoot>(20);
            _onPlayerCollision = new EventPublisher<PlayerCollision>(20);
            _onPlayerDestroyed = new EventPublisher<PlayerDestroyed>(20);
            _onPlayerLoseShield = new EventPublisher<PlayerLoseShield>(20);
            _onAsteroidsCollision = new EventPublisher<AsteroidsCollision>(20);
            _onAsteroidDestroyed = new EventPublisher<AsteroidDestroyed>(20);
            _onPickPower = new EventPublisher<PickPower>(20);
            _onMisileHit = new EventPublisher<MisileHit>(20);

        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnDestroy()
        {
            _onPlayerMove.Dispose();
            _onPlayerShoot.Dispose();
            _onPlayerCollision.Dispose();
            _onPlayerDestroyed.Dispose();
            _onPlayerLoseShield.Dispose();
            _onAsteroidsCollision.Dispose();
            _onAsteroidDestroyed.Dispose();
            _onPickPower.Dispose();
            _onMisileHit.Dispose();
            base.OnDestroy();

        }
    }
}