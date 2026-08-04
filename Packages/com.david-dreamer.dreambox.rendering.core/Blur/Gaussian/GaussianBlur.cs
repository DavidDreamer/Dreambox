using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public class GaussianBlur : IBlur
	{
		private BlurSettings Settings { get; }

		private Material Material { get; set; }

		private RTHandle RTHorizontal { get; set; }

		private RTHandle RTVertical { get; set; }

		private ComputeBuffer Kernel { get; set; }

		public RTHandle Result => RTHorizontal;

		public GaussianBlur(BlurSettings settings, GraphicsFormat graphicsFormat)
		{
			Settings = settings;

			var shaders = GraphicsSettings.GetRenderPipelineSettings<BlurShaders>();
			Material = CoreUtils.CreateEngineMaterial(shaders.Gaussian);

			int radius = Settings.KernelSize / 2;
			Material.SetInteger(BlurShaderVariable.Radius, radius);
			Material.SetFloat(BlurShaderVariable.Scale, Settings.Scale);

			Kernel = BlurUtils.CalculateGaussianKernel(Settings.KernelSize, Settings.Sigma);
			Material.SetBuffer(BlurShaderVariable.Kernel, Kernel);

			Vector2 scaleFactor = Vector2.one / Settings.Downsample;
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
					colorFormat: graphicsFormat,
					autoGenerateMips: false,
					useDynamicScale: true,
					name: $"BlurTexture_{name}"
					);
			}
		}

		public void Dispose()
		{
			CoreUtils.Destroy(Material);
			Kernel.Release();
			RTHorizontal.Release();
			RTVertical.Release();
		}

		public void Execute(CommandBuffer commandBuffer, RTHandle source)
		{
			commandBuffer.SetRenderTarget(RTHorizontal);
			Blitter.BlitTexture(commandBuffer, source, new Vector4(1, 1, 0, 0), 0, true);

			for (int i = 0; i < Settings.Iterations; i++)
			{
				Blitter.BlitTexture(commandBuffer, RTHorizontal, RTVertical, Material, BlurShaderPass.Horizontal);
				Blitter.BlitTexture(commandBuffer, RTVertical, RTHorizontal, Material, BlurShaderPass.Vertical);
			}
		}
	}
}
