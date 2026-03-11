using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Dreambox.Rendering.HDRP
{
	public static class HDRenderPipelineAssetUtils
	{
		public static HDRenderPipelineAsset CurrentPipelineAsset => (HDRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

		public static GraphicsFormat GetColorBufferGraphicsFormat()
		{
			HDRenderPipelineAsset pipelineAsset = CurrentPipelineAsset;
			var graphicsFormat = (GraphicsFormat)pipelineAsset.currentPlatformRenderPipelineSettings.colorBufferFormat;
			return graphicsFormat;
		}
	}
}
