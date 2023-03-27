using System;
using Unity.Collections;

namespace Asteroids.ECS.Events
{
    public struct EventPublisher<T> : IDisposable where T : struct
    {
        private NativeArray<T>[] Streams;
        private int[] writeIndex;
        private int[] readIndex;
        private int index;
        private int hash;
        public EventPublisher(int capacity)
        {
            Streams = new NativeArray<T>[capacity];
            writeIndex = new int[capacity];
            readIndex = new int[capacity];
            index = 0;
            hash = typeof(T).GetHashCode() + new Random().Next();
        }

        public void Dispose()
        {
            for (int i = 0; i < index; i++)
            {
                Streams[i].Dispose();
            }
        }

        public void PostEvent(T eventData)
        {
            for (int i = 0; i < index; i++)
            {
                var writer = writeIndex[i];
                ref var array = ref Streams[i];
                writeIndex[i] = (writeIndex[i] + 1) % array.Length;
                array[writer] = eventData;
            }
        }

        public EventConsumer Subscribe(int count)
        {
            int idx = index;
            Streams[index] = new NativeArray<T>(count, Allocator.Persistent);
            writeIndex[index] = 0;
            readIndex[index] = 0;
            index++;
            return new EventConsumer(idx, hash);
        }

        public bool TryGetEvent(EventConsumer consumer, out T eventData)
        {
            if (consumer.hash != hash)
                throw new ArgumentException("Invalid consumer hash, did you use the right consumer?");
            int id = consumer.id;
            var reader = readIndex[id];
            var write = writeIndex[id];

            if (reader != write)
            {
                ref var array = ref Streams[id];
                eventData = array[reader];
                readIndex[id] = (readIndex[id] + 1) % array.Length;
                return true;
            }
            else
            {
                eventData = default(T);
                return false;
            }
        }
    }

    public struct EventConsumer
    {
        public int id { get; private set; }
        public int hash { get; private set; }

        public EventConsumer(int id, int hash)
        {
            this.id = id;
            this.hash = hash;
        }
    }
}