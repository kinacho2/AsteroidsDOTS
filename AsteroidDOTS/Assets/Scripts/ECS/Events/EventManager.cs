using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.ECS.Events
{
    public static class EventManager
    {
        public static EventPublisher<AsteroidsCollision> OnAsteroidsCollision { get; private set; }

        public static void Initialize()
        {
            OnAsteroidsCollision = new EventPublisher<AsteroidsCollision>(20);
        }
        public static void Dispose()
        {
            OnAsteroidsCollision.Dispose();
        }
    }
}