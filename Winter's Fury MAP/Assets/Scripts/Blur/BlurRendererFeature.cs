using UnityEngine.Rendering.Universal;

namespace Blur
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        private BlurRenderPass blurRenderPass;

        public override void Create()
        {
            blurRenderPass = new BlurRenderPass();
            name = "Blur";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(blurRenderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            blurRenderPass.SetCameraColorTarget(renderer.cameraColorTargetHandle);
        }
    }
}