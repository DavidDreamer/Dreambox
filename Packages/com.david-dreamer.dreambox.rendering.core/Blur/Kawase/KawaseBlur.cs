using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public class KawaseBlur : IBlur
	{
		private BlurSettings Settings { get; }

		private Material Material { get; set; }

		private RTHandle RTHandle0 { get; set; }

		private RTHandle RTHandle1 { get; set; }

		public RTHandle Result { get; private set; }

		public KawaseBlur(BlurSettings settings, GraphicsFormat graphicsFormat)
		{
			Settings = settings;

			Material = CoreUtils.CreateEngineMaterial("Hidden/Dreambox/PostProcessing/Blur/Kawase");

			Material.SetFloat(BlurShaderVariable.Scale, Settings.Scale);

			Vector2 scaleFactor = Vector2.one / Settings.Downsample;
			TextureDimension dimension = TextureXR.dimension;
			int slices = TextureXR.slices;

			RTHandle0 = AllocTexture("0");
			RTHandle1 = AllocTexture("1");

			Result = Settings.Iterations % 2 == 0 ? RTHandle0 : RTHandle1;

			RTHandle AllocTexture(string name)
			{
				return RTHandles.Alloc(
					scaleFactor,
					dimension: dimension,
					slices: slices,
					colorFormat: graphicsFormat,
					autoGenerateMips: false,
					useDynamicScale: true,
					name: $"BlurTexture_{name}");
			}
		}

		public void Dispose()
		{
			CoreUtils.Destroy(Material);
			RTHandle0.Release();
			RTHandle1.Release();
		}

		public void Execute(CommandBuffer commandBuffer, RTHandle source)
		{
			commandBuffer.SetRenderTarget(RTHandle0);

			Blitter.BlitTexture(commandBuffer, source, new Vector4(1, 1, 0, 0), 0, false);

			RTHandle s = RTHandle0;
			RTHandle target = RTHandle1;

			for (int i = 0; i < Settings.Iterations; i++)
			{
				float radius = (i + 0.5f) * Settings.Scale;
				Vector2 offset = new(radius / s.rt.descriptor.width, radius / s.rt.descriptor.height);
				commandBuffer.SetGlobalVector(BlurShaderVariable.Offset, offset);

				Blitter.BlitTexture(commandBuffer, s, target, Material, BlurShaderPass.Kawase);

				(s, target) = (target, s);
			}
		}
	}
}
