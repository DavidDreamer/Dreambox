using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class BlurRenderPass : ScriptableRenderPass
	{
		private BlurRendererFeature BlurRendererFeature { get; }

		private Material Material { get; }

		public BlurRenderPass(BlurRendererFeature blurRendererFeature, Material material)
		{
			BlurRendererFeature = blurRendererFeature;
			Material = material;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			var universalResourceData = frameData.Get<UniversalResourceData>();
			var universalCameraData = frameData.Get<UniversalCameraData>();

			if (universalResourceData.isActiveTargetBackBuffer)
				return;

			TextureHandle cameraColorTexture = universalResourceData.activeColorTexture;
			RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;

			int width = cameraTargetDescriptor.width / BlurRendererFeature.Downsample;
			int height = cameraTargetDescriptor.height / BlurRendererFeature.Downsample;
			RenderTextureDescriptor renderTextureDescriptor = new(width, height, RenderTextureFormat.Default, 0);

			TextureHandle blurTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, renderTextureDescriptor, "BlurTexture", true);

			RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new(cameraColorTexture, blurTexture, Material, 0);
			renderGraph.AddBlitPass(blitMaterialParameters, "Blur.Horizontal");

			blitMaterialParameters = new(blurTexture, cameraColorTexture, Material, 1);
			renderGraph.AddBlitPass(blitMaterialParameters, "Blur.Vertical");
		}
	}
}
