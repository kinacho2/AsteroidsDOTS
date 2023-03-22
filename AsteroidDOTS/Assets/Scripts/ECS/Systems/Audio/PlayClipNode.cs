using UnityEngine;
using Unity.Audio;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using Unity.Entities;

public struct ClipStopped { }

// The 'audio job'. This is the kernel that defines a running DSP node inside the
// DSPGraph. It is a struct that implements the IAudioKernel interface. It can contain
// internal state, and will have the Execute function called as part of the graph
// traversal during an audio frame.
[BurstCompile(CompileSynchronously = true)]
struct PlayClipNode : IAudioKernel<PlayClipNode.Parameters, PlayClipNode.SampleProviders>
{
    // Parameters are currently defined with enumerations. Each enum value corresponds to
    // a parameter within the node. Setting a value for a parameter uses these enum values.
    public enum Parameters { Rate }

    // Sample providers are defined with enumerations. Each enum value defines a slot where
    // a sample provider can live on a IAudioKernel. Sample providers are used to get samples from
    // AudioClips and VideoPlayers. They will eventually be able to pull samples from microphones and other concepts.
    public enum SampleProviders { DefaultSlot }

    // The clip sample rate might be different to the output rate used by the system. Therefore we use a resampler
    // here.
    public Resampler Resampler;

    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float> ResampleBuffer;

    public bool Playing;

    public void Initialize()
    {
        // During an initialization phase, we have access to a resource context which we can
        // do buffer allocations with safely in the job.
        ResampleBuffer = new NativeArray<float>(1025 * 2, Allocator.AudioKernel);

        // set position to "end of buffer", to force pulling data on first iteration
        Resampler.Position = (double)ResampleBuffer.Length / 2;
    }

    public void Execute(ref ExecuteContext<Parameters, SampleProviders> context)
    {
        if (Playing)
        {
            // During the creation phase of this node we added an output port to feed samples to.
            // This API gives access to that output buffer.
            var buffer = context.Outputs.GetSampleBuffer(0);

            // Get the sample provider for the AudioClip currently being played. This allows
            // streaming of samples from the clip into a buffer.
            var provider = context.Providers.GetSampleProvider(SampleProviders.DefaultSlot);

            // We pass the provider to the resampler. If the resampler finishes streaming all the samples, it returns
            // true.
            var finished = Resampler.ResampleLerpRead(provider, ResampleBuffer, buffer.GetBuffer(1), context.Parameters, Parameters.Rate);
            Resampler.ResampleLerpRead(provider, ResampleBuffer, buffer.GetBuffer(0), context.Parameters, Parameters.Rate);
            if (finished)
            {
                // Post an async event back to the main thread, telling the handler that the clip has stopped playing.
                context.PostEvent(new ClipStopped());
                Playing = false;
            }
        }
    }

    public void Dispose()
    {
        if (ResampleBuffer.IsCreated)
            ResampleBuffer.Dispose();
    }
}
