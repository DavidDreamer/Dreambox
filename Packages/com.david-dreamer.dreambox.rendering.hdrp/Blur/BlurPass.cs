using System.Collections.Generic;
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
		[field: Range(1, 8)]
		private int Downsample { get; set; } = 2;

		[field: SerializeField]
		[field: Range(1, 256)]
		private int Radius { get; set; } = 16;

		[field: SerializeField]
		[field: Range(1f, 5f)]
		private float Sigma { get; set; } = 1f;

		[field: SerializeField]
		private OutputTarget Target { get; set; }

		private Material Material { get; set; }

		private RTHandle RTHorizontal { get; set; }

		private RTHandle RTVertical { get; set; }

		private ComputeBuffer GaussianWeights { get; set; }

		protected override bool executeInSceneView => false;

		public override IEnumerable<Material> RegisterMaterialForInspector()
		{
			yield return Material;
		}

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Material = CoreUtils.CreateEngineMaterial("Hidden/Dreambox/PostProcessing/Blur");

			Material.SetFloat(BlurShaderVariable.Radius, Radius);

			GaussianWeights = BlurUtils.CalculateGaussianWeights(Radius, Sigma * Sigma);
			Material.SetBuffer(BlurShaderVariable.GaussianWeights, GaussianWeights);

			Vector2 scaleFactor = Vector2.one / Downsample;
			GraphicsFormat colorFormat = HDRenderPipelineAssetUtils.GetColorBufferGraphicsFormat();
			TextureDimension dimension = TextureXR.dimension;
			int slices = TextureXR.slices;

			RTHorizontal = AllocTexture("Horizontal");
			RTVertical = AllocTexture("Vertical");

			RTHandle AllocTexture(string name)
			{
				return RTHandles.Alloc(
					scaleFactor,
					dimension: dimension,
					slices: slices,
					colorFormat: colorFormat,
					autoGenerateMips: false,
					useDynamicScale: true,
					name: $"{TextureName}_{name}"
					);
			}
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(Material);
			GaussianWeights.Release();
			RTHorizontal.Release();
			RTVertical.Release();
		}

		protected override void Execute(CustomPassContext context)
		{
			base.Execute(context);

			CommandBuffer commandBuffer = context.cmd;

			commandBuffer.SetRenderTarget(RTHorizontal);
			Blitter.BlitTexture(commandBuffer, context.cameraColorBuffer, new Vector4(1, 1, 0, 0), 0, false);

			Blitter.BlitTexture(commandBuffer, RTHorizontal, RTVertical, Material, BlurShaderPass.Horizontal);
			Blitter.BlitTexture(commandBuffer, RTVertical, RTHorizontal, Material, BlurShaderPass.Vertical);

			switch (Target)
			{
				case OutputTarget.Camera:
					commandBuffer.SetRenderTarget(context.cameraColorBuffer);
					Blitter.BlitTexture(commandBuffer, RTHorizontal, new Vector4(1, 1, 0, 0), 0, false);
					break;
				case OutputTarget.Texture:
					commandBuffer.SetGlobalTexture(TextureName, RTHorizontal);
					break;
			}
		}
	}
}
