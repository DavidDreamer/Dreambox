using System.Collections.Generic;
using System.Diagnostics;
using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Dreambox.Rendering.HDRP
{
	public class BlurPass : CustomPass
	{
		public enum OutputTarget
		{
			Camera,
			Texture
		}

		private const string TextureName = "BlurTexture";

		[field: SerializeField]
		[field: Range(1, 8)]
		public int Downsample { get; private set; } = 2;

		[field: SerializeField]
		[field: Range(3, 51)]
		public int KernelSize { get; private set; } = 15;

		[field: SerializeField]
		[field: Range(1, 10)]
		public float Scale { get; private set; } = 1;

		[field: SerializeField]
		[field: Range(1f, 10f)]
		public float Sigma { get; private set; } = 3f;

		[field: SerializeField]
		public OutputTarget Target { get; private set; }

		private Material Material { get; set; }

		private RTHandle RTHorizontal { get; set; }

		private RTHandle RTVertical { get; set; }

		private ComputeBuffer Kernel { get; set; }

		protected override bool executeInSceneView => false;

		public override IEnumerable<Material> RegisterMaterialForInspector()
		{
			yield return Material;
		}

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Setup();
		}

		private void Setup()
		{
			Material = CoreUtils.CreateEngineMaterial("Hidden/Dreambox/PostProcessing/Blur");

			float radius = KernelSize / 2;
			Material.SetFloat(BlurShaderVariable.Radius, radius);
			Material.SetFloat(BlurShaderVariable.Scale, Scale);

			Kernel = BlurUtils.CalculateGaussianKernel(KernelSize, Sigma);
			Material.SetBuffer(BlurShaderVariable.Kernel, Kernel);

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
			Kernel.Release();
			RTHorizontal.Release();
			RTVertical.Release();
		}

		[Conditional("UNITY_EDITOR")]
		public void Reset()
		{
			Cleanup();
			Setup();
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
					commandBuffer.SetGlobalTexture(BlurShaderVariable.BlurTexture, RTHorizontal);
					break;
			}
		}
	}
}
