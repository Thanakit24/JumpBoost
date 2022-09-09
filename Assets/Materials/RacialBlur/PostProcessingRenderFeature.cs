using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public bool isEnabled = true;
        //Doing After is buggy
        public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int materialPassIndex = -1; // -1 means render all passes
        public Material material;
    }

    [SerializeField] public Settings settings = new Settings();
    private RenderPass renderPass;

    public override void Create()
    {
        //Debug.Log($"printing name of shader{name}");
        renderPass = new RenderPass(
            name,
            settings.material,
            settings.materialPassIndex
        );
        
        renderPass.renderPassEvent = settings.renderEvent;
    }

    // called every frame once per camera
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.isEnabled)
        {
            // we can do nothing this frame if we want
            return;
        }

        renderPass.SetSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(renderPass);
    }


    class RenderPass : ScriptableRenderPass
    {
        private string profilingName;
        private Material material;
        private int materialPassIndex;
        private RenderTargetIdentifier sourceID;
        private RenderTargetHandle tempTextureHandle;

        public RenderPass(string profilingName, Material material, int passIndex) : base()
        {
            this.profilingName = profilingName;
            this.material = material;
            this.materialPassIndex = passIndex;
            // create a temporary render texture that matches the camera
            tempTextureHandle.Init("_TempBlitMaterialTexture");
        }

        // This isn't part of the ScriptableRenderPass class and is our own addition.
        // For this custom pass we need the camera's color target, so that gets passed in.
        public void SetSource(RenderTargetIdentifier source)
        {
            this.sourceID = source;
        }

        // Execute is called for every eligible camera every frame. It's not called at the moment that
        // rendering is actually taking place, so don't directly execute rendering commands here.
        // Instead use the methods on ScriptableRenderContext to set up instructions.
        // RenderingData provides a bunch of (not very well documented) information about the scene
        // and what's being rendered.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // fetch a command buffer to use
            CommandBuffer cmd = CommandBufferPool.Get(profilingName);

            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTextureHandle.id, cameraTextureDesc, FilterMode.Bilinear);
            // the actual content of our custom render pass!
            // we apply our material while blitting to a temporary texture
            Blit(cmd, sourceID, tempTextureHandle.Identifier(), material, materialPassIndex);
            // ...then blit it back again 
            Blit(cmd, tempTextureHandle.Identifier(), sourceID);

            context.ExecuteCommandBuffer(cmd);
            // tidy up after ourselves
            CommandBufferPool.Release(cmd);
        }

        // called after Execute, use it to clean up anything allocated in Configure
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTextureHandle.id);
        }
    }
}