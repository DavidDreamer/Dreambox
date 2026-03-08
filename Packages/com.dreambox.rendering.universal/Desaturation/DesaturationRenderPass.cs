using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class DesaturationRenderPass : ScriptableRenderPass
	{
		private Material Material { get; }

		public DesaturationRenderPass(Material material)
		{
			Material = material;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			var universalResourceData = frameData.Get<UniversalResourceData>();

			TextureHandle cameraColorTexture = universalResourceData.activeColorTexture;

			RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new(cameraColorTexture, cameraColorTexture, Material, 0);
			renderGraph.AddBlitPass(blitMaterialParameters, "Desaturation");
		}
	}
}
