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
			CommandBuffer commandBuffer = CommandBufferPool.Get(nameof(BlurRendererFeature));

			RTHandle cameraColorTargetHandler = renderingData.cameraData.renderer.cameraColorTargetHandle;

			commandBuffer.EnableKeyword(BlurRendererFeature.Material,
				new LocalKeyword(BlurRendererFeature.Material.shader,
					BlurRendererFeature.Settings.Algorithm.ToShaderName()));

			BlurRendererFeature.Material.SetInt(BlurShaderVariables.Radius, BlurRendererFeature.Settings.Radius);
			BlurRendererFeature.Material.SetFloat(BlurShaderVariables.Factor, BlurRendererFeature.Factor);

			commandBuffer.Blit(cameraColorTargetHandler, BlurRendererFeature.tempTexture, BlurRendererFeature.Material,
				0);
			commandBuffer.Blit(BlurRendererFeature.tempTexture, cameraColorTargetHandler, BlurRendererFeature.Material,
				1);

			context.ExecuteCommandBuffer(commandBuffer);

			commandBuffer.Clear();
			CommandBufferPool.Release(commandBuffer);
		}
	}
}
