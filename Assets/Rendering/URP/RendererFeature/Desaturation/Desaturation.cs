using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public class Desaturation : PostProcessRenderFeature<DesaturationRenderPass>
	{
		public RTHandle tempTexture;

		public float Factor { get; private set; }

		public override void Create()
		{
			base.Create();

			Pass.RendererFeature = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			tempTexture?.Release();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			var volumeComponent = VolumeManager.instance.stack.GetComponent<DesaturationVolumeComponent>();

			if (volumeComponent is null || volumeComponent.active is false)
			{
				return;
			}

			Factor = volumeComponent.Factor.value;

			CameraData cameraData = renderingData.cameraData;
			if (cameraData.isSceneViewCamera || cameraData.isPreviewCamera)
			{
				return;
			}

			renderer.EnqueuePass(Pass);
		}
	}
}
