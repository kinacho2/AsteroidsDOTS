using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public struct EventPublisher<T> : IDisposable where T : struct
{
    private NativeArray<T>[] Streams;
    private int[] writeIndex;
    private int[] readIndex;
    private int index;
    public EventPublisher(int capacity)
    {
        Streams = new NativeArray<T>[capacity];
        writeIndex = new int[capacity];
        readIndex = new int[capacity];
        index = 0;
    }
    public void Dispose()
    {
        for(int i = 0; i < index; i++)
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

    public int Subscribe(int count)
    {
        int idx = index;
        Streams[index] = new NativeArray<T>(count, Allocator.Persistent);
        writeIndex[index] = 0;
        readIndex[index] = 0;
        index++;
        return idx;
    }

    public bool TryGetEvent(int consumer, out T eventData)
    {
        var reader = readIndex[consumer];
        var write = writeIndex[consumer];

        if (reader != write)
        {
            ref var array = ref Streams[consumer];
            eventData = array[reader];
            readIndex[consumer] = (readIndex[consumer] + 1) % array.Length;
            return true;
        }
        else
        {
            eventData = default(T);
            return false;
        }
    }
}
