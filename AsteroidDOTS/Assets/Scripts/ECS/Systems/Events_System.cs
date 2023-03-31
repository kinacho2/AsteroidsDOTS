using Asteroids.ECS.Events;
using Asteroids.Setup;
using Unity.Entities;

namespace Asteroids.ECS.Systems
{
    //[UpdateInGroup(typeof(InitializationSystemGroup))]
    
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

        private static EventPublisher<MissileHit> _onMissileHit;
        public static ref EventPublisher<MissileHit> OnMissileHit => ref _onMissileHit;

        private static EventPublisher<PlayerMove> _onHyperspaceTravelStart;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravelStart => ref _onHyperspaceTravelStart;

        private static EventPublisher<PlayerMove> _onHyperspaceTravelStop;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravelStop => ref _onHyperspaceTravelStop;

        private static EventPublisher<PlayerMove> _onHyperspaceTravel;
        public static ref EventPublisher<PlayerMove> OnHyperspaceTravel => ref _onHyperspaceTravel;
        protected override void OnCreate()
        {
            base.OnCreate();
            _onPlayerStartMove = new EventPublisher<PlayerMove>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPlayerStoptMove = new EventPublisher<PlayerMove>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPlayerShoot = new EventPublisher<EntityShoot>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPlayerCollision = new EventPublisher<PlayerCollision>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPlayerDestroyed = new EventPublisher<EntityDestroyed>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPlayerLoseShield = new EventPublisher<PlayerLoseShield>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onAsteroidsCollision = new EventPublisher<AsteroidsCollision>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onAsteroidDestroyed = new EventPublisher<AsteroidDestroyed>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onPickPower = new EventPublisher<PickPower>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onMissileHit = new EventPublisher<MissileHit>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onHyperspaceTravelStart = new EventPublisher<PlayerMove>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onHyperspaceTravelStop = new EventPublisher<PlayerMove>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
            _onHyperspaceTravel = new EventPublisher<PlayerMove>(Configs.EVENTS_STREAM_CAPACITY, Configs.EVENTS_QUEUE_COUNT);
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
            _onMissileHit.Dispose();
            _onPlayerStoptMove.Dispose();
            _onHyperspaceTravelStart.Dispose();
            _onHyperspaceTravelStop.Dispose();
            _onHyperspaceTravel.Dispose();
            base.OnDestroy();
        }
    }
}