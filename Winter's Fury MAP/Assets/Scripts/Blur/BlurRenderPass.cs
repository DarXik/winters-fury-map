using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    private Material material;
    private BlurSettings blurSettings;

    private RenderTargetIdentifier source;
    private RTHandle renderTarget;

    public bool Setup(ScriptableRenderer renderer)
    {
        source = renderer.cameraColorTargetHandle;
        blurSettings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        if (blurSettings != null && blurSettings.IsActive())
        {
            material = new Material(Shader.Find("PostProcessing/Blur"));
            return true;
        }

        return false;
    }

    public void SetCameraColorTarget(RenderTargetIdentifier cameraColorTargetIdent)
    {
        source = cameraColorTargetIdent;
        blurSettings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        material = new Material(Shader.Find("PostProcessing/Blur"));
    }
    
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if (blurSettings == null || !blurSettings.IsActive())
        {
            return;
        }

        renderTarget = RTHandles.Alloc("_BlurTex", name: "_BlurTex");
        
        cmd.GetTemporaryRT(Shader.PropertyToID(renderTarget.name), cameraTextureDescriptor);
        
        base.Configure(cmd, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blurSettings == null || !blurSettings.IsActive())
        {
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("Blur Post Process");

        int gridSize = Mathf.CeilToInt(blurSettings.strength.value * 6f);

        if (gridSize % 2 == 0)
        {
            gridSize++;
        }
        
        material.SetInteger("_GridSize", gridSize);
        material.SetFloat("_Spread", blurSettings.strength.value);
        
        cmd.Blit(source, Shader.PropertyToID(renderTarget.name), material, 0);
        cmd.Blit(Shader.PropertyToID(renderTarget.name), source, material, 1);
        context.ExecuteCommandBuffer(cmd);
        
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
