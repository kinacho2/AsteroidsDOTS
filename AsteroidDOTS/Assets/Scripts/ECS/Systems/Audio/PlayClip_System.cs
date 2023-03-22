using Asteroids.Setup;
using Unity.Audio;
using Unity.Entities;
using UnityEngine;

public class PlayClip_System : SystemBase
{
    public AudioClip ClipToPlay;

    private AudioOutputHandle m_Output;
    private static DSPGraph m_Graph;
    private static DSPNode[] m_Node;
    private static int _nodeIndex;
    private DSPConnection m_Connection;

    private int m_HandlerID;

    protected override void OnCreate()
    {
        base.OnCreate();
        InitializeAudioSystem();
    }

    private void InitializeAudioSystem()
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);
        AudioSettings.GetDSPBufferSize(out var bufferLength, out var numBuffers);

        var sampleRate = AudioSettings.outputSampleRate;

        m_Graph = DSPGraph.Create(format, channels, bufferLength, sampleRate);

        var driver = new DefaultDSPGraphDriver { Graph = m_Graph };
        m_Output = driver.AttachToDefaultOutput();

        // Add an event handler delegate to the graph for ClipStopped. So we are notified
        // of when a clip is stopped in the node and can handle the resources on the main thread.
        m_HandlerID = m_Graph.AddNodeEventHandler<ClipStopped>((node, evt) =>
        {
            Debug.Log("Received ClipStopped event on main thread, cleaning resources");
        });

        // All async interaction with the graph must be done through a DSPCommandBlock.
        // Create it here and complete it once all commands are added.
        var block = m_Graph.CreateCommandBlock();

        //probably needs to add an array
        m_Node = new DSPNode[Configs.AUDIO_PLAY_COUNT];
        for(int i=0; i< m_Node.Length; i++)
        {
            m_Node[i] = block.CreateDSPNode<PlayClipNode.Parameters, PlayClipNode.SampleProviders, PlayClipNode>();

            // Currently input and output ports are dynamic and added via this API to a node.
            // This will change to a static definition of nodes in the future.
            block.AddOutletPort(m_Node[i], 2);

            // Connect the node to the root of the graph.
            m_Connection = block.Connect(m_Node[i], 0, m_Graph.RootDSP, 0);
        }
        _nodeIndex = 0;
        // We are done, fire off the command block atomically to the mixer thread.
        block.Complete();
    }

    protected override void OnUpdate()
    {
        m_Graph.Update();
    }

    protected override void OnDestroy()
    {
        // Command blocks can also be completed via the C# 'using' construct for convenience
        using (var block = m_Graph.CreateCommandBlock())
        {
            block.Disconnect(m_Connection);
            foreach (var node in m_Node)
            {
                block.ReleaseDSPNode(node);
            }
        }

        m_Graph.RemoveNodeEventHandler(m_HandlerID);

        m_Output.Dispose();
        base.OnDestroy();
    }

    public static void PlayClip(AudioClip clip)
    {

        if (clip == null)
        {
            return;
        }

        using (var block = m_Graph.CreateCommandBlock())
        {
            // Decide on playback rate here by taking the provider input rate and the output settings of the system
            var resampleRate = (float)clip.frequency / AudioSettings.outputSampleRate;
            var node = m_Node[_nodeIndex];
            block.SetFloat<PlayClipNode.Parameters, PlayClipNode.SampleProviders, PlayClipNode>(node, PlayClipNode.Parameters.Rate, resampleRate);

            // Assign the sample provider to the slot of the node.
            block.SetSampleProvider<PlayClipNode.Parameters, PlayClipNode.SampleProviders, PlayClipNode>(clip, node, PlayClipNode.SampleProviders.DefaultSlot);

            // Kick off playback. This will be done in a better way in the future.
            block.UpdateAudioKernel<PlayClipKernel, PlayClipNode.Parameters, PlayClipNode.SampleProviders, PlayClipNode>(new PlayClipKernel(), node);

            _nodeIndex = (_nodeIndex + 1) % m_Node.Length;
        }
        
    }


}