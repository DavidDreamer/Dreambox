using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public class BlurRenderPass: ScriptableRenderPass
	{
		public Blur BlurRendererFeature { get; set; }

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor cameraColorTargetDescriptor =
				renderingData.cameraData.renderer.cameraColorTargetHandle.rt.descriptor;

			Vector2 scaleFactor = Vector2.one / BlurRendererFeature.Settings.Downsample;

			RenderingUtils.ReAllocateIfNeeded(ref BlurRendererFeature.tempTexture, scaleFactor,
				in cameraColorTargetDescriptor,
				name: "TempBlurTexture");
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			var scope = new CommandBufferContextScope(context, nameof(BlurRendererFeature));
			CommandBuffer commandBuffer = scope.CommandBuffer;

			RTHandle cameraColorTargetHandler = renderingData.cameraData.renderer.cameraColorTargetHandle;

			BlurRendererFeature.Settings.ApplyTo(BlurRendererFeature.Material);

			commandBuffer.Blit(cameraColorTargetHandler, BlurRendererFeature.tempTexture, BlurRendererFeature.Material,
				0);
			commandBuffer.Blit(BlurRendererFeature.tempTexture, cameraColorTargetHandler, BlurRendererFeature.Material,
				1);
		}
	}
}
