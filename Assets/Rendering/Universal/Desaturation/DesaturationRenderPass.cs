using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class DesaturationRenderPass : ScriptableRenderPass
	{
		public Desaturation RendererFeature { get; set; }

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			RenderTextureDescriptor cameraColorTargetDescriptor =
				renderingData.cameraData.renderer.cameraColorTargetHandle.rt.descriptor;

			RenderingUtils.ReAllocateIfNeeded(ref RendererFeature.tempTexture, Vector2.one,
				in cameraColorTargetDescriptor,
				name: "TempTexture");
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer commandBuffer = CommandBufferPool.Get(nameof(Desaturation));

			RTHandle cameraColorTargetHandler = renderingData.cameraData.renderer.cameraColorTargetHandle;

			RendererFeature.Material.SetFloat(DesaturationShaderVariables.Factor, RendererFeature.Factor);

			commandBuffer.Blit(cameraColorTargetHandler, RendererFeature.tempTexture,
				RendererFeature.Material);

			commandBuffer.Blit(RendererFeature.tempTexture, cameraColorTargetHandler);

			context.ExecuteCommandBuffer(commandBuffer);

			commandBuffer.Clear();
			CommandBufferPool.Release(commandBuffer);
		}
	}
}
