using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Dreambox.Rendering.HDRP
{
	public class BlurPass : CustomPass
	{
		private enum OutputTarget
		{
			Camera,
			Texture
		}

		private const string TextureName = "BlurTexture";

		[field: SerializeField]
		private Material Material { get; set; }

		[field: SerializeField]
		[field: Range(1, 4)]
		private int Downsample { get; set; } = 1;

		[field: SerializeField]
		private OutputTarget Target { get; set; }

		private RTHandle Texture1 { get; set; }

		private RTHandle Texture2 { get; set; }

		protected override bool executeInSceneView => false;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Vector2 scaleFactor = Vector2.one / Downsample;
			GraphicsFormat colorFormat = HDRenderPipelineAssetUtils.GetColorBufferGraphicsFormat();
			TextureDimension dimension = TextureXR.dimension;

			Texture1 = RTHandles.Alloc(
				scaleFactor,
				dimension: dimension,
				colorFormat: colorFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				name: $"{TextureName}_Horizontal"
			);

			Texture2 = RTHandles.Alloc(
				scaleFactor,
				dimension: dimension,
				colorFormat: colorFormat,
				autoGenerateMips: false,
				useDynamicScale: true,
				name: $"{TextureName}_Vertical"
			);
		}

		protected override void Cleanup()
		{
			Texture1.Release();
			Texture2.Release();
		}

		protected override void Execute(CustomPassContext ctx)
		{
			base.Execute(ctx);

			CommandBuffer commandBuffer = ctx.cmd;

			Blitter.BlitTexture(commandBuffer, ctx.cameraColorBuffer, Texture2, Material, BlurShaderPass.Horizontal);
			Blitter.BlitTexture(commandBuffer, Texture2, Texture1, Material, BlurShaderPass.Vertical);

			switch (Target)
			{
				case OutputTarget.Camera:
					commandBuffer.SetRenderTarget(ctx.cameraColorBuffer);
					Blitter.BlitTexture(commandBuffer, Texture1, new Vector4(1, 1, 0, 0), 0, false);
					break;
				case OutputTarget.Texture:
					commandBuffer.SetGlobalTexture(TextureName, Texture1);
					break;
			}
		}
	}
}
