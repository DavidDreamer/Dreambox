using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public class Blur : PostProcessRenderFeature<BlurRenderPass>
	{
		[field: SerializeField]
		public BlurSettings Settings { get; private set; }

		public RTHandle tempTexture;

		public float Factor { get; private set; }

		public override void Create()
		{
			base.Create();

			Pass.BlurRendererFeature = this;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			tempTexture?.Release();
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			var blurVolumeComponent = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();

			if (blurVolumeComponent is null || blurVolumeComponent.active is false)
			{
				return;
			}

			Factor = blurVolumeComponent.Factor.value;

			CameraData cameraData = renderingData.cameraData;
			if (cameraData.isSceneViewCamera || cameraData.isPreviewCamera)
			{
				return;
			}

			renderer.EnqueuePass(Pass);
		}
	}
}
