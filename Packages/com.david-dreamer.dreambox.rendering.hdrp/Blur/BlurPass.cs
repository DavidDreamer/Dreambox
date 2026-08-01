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
		public OutputTarget Target { get; private set; }

		[field: SerializeField]
		public BlurSettings Settings { get; private set; }

		private Material Material { get; set; }

		private RTHandle RTHorizontal { get; set; }

		private RTHandle RTVertical { get; set; }

		private ComputeBuffer Kernel { get; set; }

		protected override bool executeInSceneView => false;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Setup();
		}

		private void Setup()
		{
			name = $"Blur - {Settings.Mode}";

			Material = CoreUtils.CreateEngineMaterial("Hidden/Dreambox/PostProcessing/Blur");

			float radius = Settings.KernelSize / 2;
			Material.SetFloat(BlurShaderVariable.Radius, radius);
			Material.SetFloat(BlurShaderVariable.Scale, Settings.Scale);

			Kernel = BlurUtils.CalculateGaussianKernel(Settings.KernelSize, Settings.Sigma);
			Material.SetBuffer(BlurShaderVariable.Kernel, Kernel);

			Vector2 scaleFactor = Vector2.one / Settings.Downsample;
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

			for (int i = 0; i < Settings.Iterations; i++)
			{
				Blitter.BlitTexture(commandBuffer, RTHorizontal, RTVertical, Material, BlurShaderPass.Horizontal);
				Blitter.BlitTexture(commandBuffer, RTVertical, RTHorizontal, Material, BlurShaderPass.Vertical);
			}

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
