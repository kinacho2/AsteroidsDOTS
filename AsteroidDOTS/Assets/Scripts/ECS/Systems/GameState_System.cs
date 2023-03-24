using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.ECS.Events;
using Asteroids.Setup;
using Unity.Collections;
using Unity.Entities;

namespace Asteroids.ECS.Systems
{
    public class GameState_System : SystemBase
    {
        private EventConsumer _eventConsumer;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            var entity = EntityManager.CreateEntity();
            EntityManager.SetName(entity, "Game State");
            EntityManager.AddComponentData(entity, new GameStateComponent { state = GameState.Running });

            _eventConsumer = Events_System.OnEntityDestroyed.Subscribe(Configs.EVENTS_QUEUE_COUNT);
        }

        protected override void OnUpdate()
        {
            EntityQuery query = GetEntityQuery(typeof(GameStateComponent));
            var gameStateEntity = query.GetSingletonEntity();

            if (Events_System.OnEntityDestroyed.TryGetEvent(_eventConsumer, out var destroyed))
            {
                if (destroyed.entityType == EntityType.Player)
                {
                    var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

                    var player = GetEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();

                    var gameState = EntityManager.GetComponentData<GameStateComponent>(gameStateEntity);
                    gameState.state = GameState.Finished;
                    cmdBuffer.SetComponent(gameStateEntity, gameState);
                    cmdBuffer.DestroyEntity(player);
                    cmdBuffer.Playback(EntityManager);
                    cmdBuffer.Dispose();

                }
            }


        }

    }

    public struct GameStateComponent : IComponentData
    {
        public GameState state;
    }

    public enum GameState
    {
        Running,
        Finished
    }
}