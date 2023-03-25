using Asteroids.ECS.Events;
using Unity.Entities;

namespace Asteroids.ECS.Systems
{
    public class Events_System : SystemBase
    {
        private static EventPublisher<PlayerMove> _onPlayerStartMove;
        public static ref EventPublisher<PlayerMove> OnPlayerStartMove => ref _onPlayerStartMove;

        private static EventPublisher<PlayerMove> _onPlayerStoptMove;
        public static ref EventPublisher<PlayerMove> OnPlayerStopMove => ref _onPlayerStoptMove;

        private static EventPublisher<EntityShoot> _onPlayerShoot;
        public static ref EventPublisher<EntityShoot> OnEntityShoot => ref _onPlayerShoot;

        private static EventPublisher<PlayerCollision> _onPlayerCollision;
        public static ref EventPublisher<PlayerCollision> OnPlayerCollision => ref _onPlayerCollision;

        private static EventPublisher<EntityDestroyed> _onPlayerDestroyed;
        public static ref EventPublisher<EntityDestroyed> OnEntityDestroyed => ref _onPlayerDestroyed;
        
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

        private static EventPublisher<PlayerMove> _onHyperspaceTravelStart;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravelStart => ref _onHyperspaceTravelStart;

        private static EventPublisher<PlayerMove> _onHyperspaceTravelStop;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravelStop => ref _onHyperspaceTravelStop;

        private static EventPublisher<PlayerMove> _onHyperspaceTravel;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravel => ref _onHyperspaceTravel;
        protected override void OnCreate()
        {
            base.OnCreate();
            _onPlayerStartMove = new EventPublisher<PlayerMove>(20);
            _onPlayerStoptMove = new EventPublisher<PlayerMove>(20);
            _onPlayerShoot = new EventPublisher<EntityShoot>(20);
            _onPlayerCollision = new EventPublisher<PlayerCollision>(20);
            _onPlayerDestroyed = new EventPublisher<EntityDestroyed>(20);
            _onPlayerLoseShield = new EventPublisher<PlayerLoseShield>(20);
            _onAsteroidsCollision = new EventPublisher<AsteroidsCollision>(20);
            _onAsteroidDestroyed = new EventPublisher<AsteroidDestroyed>(20);
            _onPickPower = new EventPublisher<PickPower>(20);
            _onMisileHit = new EventPublisher<MisileHit>(20);

            _onHyperspaceTravelStart = new EventPublisher<PlayerMove>(20);
            _onHyperspaceTravelStop = new EventPublisher<PlayerMove>(20);
            _onHyperspaceTravel = new EventPublisher<PlayerMove>(20);
            
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnDestroy()
        {
            _onPlayerStartMove.Dispose();
            _onPlayerShoot.Dispose();
            _onPlayerCollision.Dispose();
            _onPlayerDestroyed.Dispose();
            _onPlayerLoseShield.Dispose();
            _onAsteroidsCollision.Dispose();
            _onAsteroidDestroyed.Dispose();
            _onPickPower.Dispose();
            _onMisileHit.Dispose();
            _onPlayerStoptMove.Dispose();
            _onHyperspaceTravelStart.Dispose();
            _onHyperspaceTravelStop.Dispose();
            _onHyperspaceTravel.Dispose();
            base.OnDestroy();

        }
    }
}