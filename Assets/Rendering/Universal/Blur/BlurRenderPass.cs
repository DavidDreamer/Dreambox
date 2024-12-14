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
		private BlurSettings Settings { get; }

		private Material Material { get; }

		public BlurRenderPass(BlurSettings settings, Material material)
		{
			Settings = settings;
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

			int width = cameraTargetDescriptor.width / Settings.Downsample;
			int height = cameraTargetDescriptor.width / Settings.Downsample;
			RenderTextureDescriptor renderTextureDescriptor = new(width, height, RenderTextureFormat.Default, 0);

			TextureHandle blurTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, renderTextureDescriptor, "BlurTexture", true);

			Settings.ApplyTo(Material);

			RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new(cameraColorTexture, blurTexture, Material, 0);
			renderGraph.AddBlitPass(blitMaterialParameters, "Blur.Horizontal");

			blitMaterialParameters = new(blurTexture, cameraColorTexture, Material, 1);
			renderGraph.AddBlitPass(blitMaterialParameters, "Blur.Vertical");
		}
	}
}
