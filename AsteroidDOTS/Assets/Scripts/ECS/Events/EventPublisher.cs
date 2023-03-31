using System;
using Unity.Collections;

namespace Asteroids.ECS.Events
{
    public struct EventPublisher<T> : IDisposable where T : struct
    {
        private NativeArray<T> Streams;
        private NativeArray<int> writeIndex;
        private NativeArray<int> readIndex;

        private int index;
        private long hash;

        private int events;
        public EventPublisher(int capacity, int events)
        {
            Streams = new NativeArray<T>(capacity * events, Allocator.Persistent);
            writeIndex = new NativeArray<int>(capacity, Allocator.Persistent);
            readIndex = new NativeArray<int>(capacity, Allocator.Persistent);
            index = 0;
            hash = ((long)typeof(T).GetHashCode()) + new Random().Next();

            this.events = events;
        }

        public void Dispose()
        {
            Streams.Dispose();
            writeIndex.Dispose();
            readIndex.Dispose();
        }

        public void PostEvent(T eventData)
        {
            for (int i = 0; i < index; i++)
            {
                var writer = writeIndex[i];
                writeIndex[i] = (writer + 1) % events + i * events;
                Streams[writer] = eventData;
            }
        }

        public EventConsumer Subscribe()
        {
            int idx = index;
            writeIndex[index] = events * idx;
            readIndex[index] = events * idx;
            index++;
            return new EventConsumer(idx, hash);
        }

        public bool TryGetEvent(EventConsumer consumer, out T eventData)
        {
            if (consumer.hash != hash)
                throw new ArgumentException($"Invalid consumer hash, ({hash},{consumer.hash}) did you use the right consumer?");
            
            int id = consumer.id;
            var reader = readIndex[id];
            var write = writeIndex[id];

            if (reader != write)
            {
                eventData = Streams[reader];
                readIndex[id] = (reader + 1) % events + id * events;
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
        internal long hash { get; private set; }

        internal EventConsumer(int id, long hash)
        {
            this.id = id;
            this.hash = hash;
        }
    }
}