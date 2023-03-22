using Unity.Audio;
using Unity.Burst;

[BurstCompile(CompileSynchronously = true)]
struct PlayClipKernel : IAudioKernelUpdate<PlayClipNode.Parameters, PlayClipNode.SampleProviders, PlayClipNode>
{
    // This update job is used to kick off playback of the node.
    public void Update(ref PlayClipNode audioKernel)
    {
        audioKernel.Playing = true;
    }
}


