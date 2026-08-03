using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Dreambox.Rendering.Core
{
	public class DualKawaseBlur : IBlur
	{
		private BlurSettings Settings { get; }

		private Material Material { get; set; }

		private RTHandle[] Textures { get; set; }

		public RTHandle Result => Textures[0];

		public DualKawaseBlur(BlurSettings settings, GraphicsFormat graphicsFormat)
		{
			Settings = settings;

			var shaders = GraphicsSettings.GetRenderPipelineSettings<BlurShaders>();
			Material = CoreUtils.CreateEngineMaterial(shaders.DualKawase);

			TextureDimension dimension = TextureXR.dimension;
			int slices = TextureXR.slices;

			Textures = new RTHandle[Settings.Downsample];

			for (int i = 0; i < Textures.Length; i++)
			{
				Textures[i] = AllocTexture(i);
			}

			RTHandle AllocTexture(int index)
			{
				return RTHandles.Alloc(
					Vector2.one / (index + 1),
					dimension: dimension,
					slices: slices,
					colorFormat: graphicsFormat,
					autoGenerateMips: false,
					useDynamicScale: true,
					name: $"{nameof(DualKawaseBlur)}_{index}");
			}
		}

		public void Dispose()
		{
			CoreUtils.Destroy(Material);

			for (int i = 0; i < Textures.Length; i++)
			{
				Textures[i].Release();
			}
		}

		public void Execute(CommandBuffer commandBuffer, RTHandle source)
		{
			commandBuffer.SetRenderTarget(Textures[0]);

			Blitter.BlitTexture(commandBuffer, source, new Vector4(1, 1, 0, 0), 0, true);

			float radius = 0.5f * Settings.Scale;

			for (int i = 0; i < Textures.Length - 1; i++)
			{
				Vector2 offset = new(radius / Textures[i].rt.descriptor.width, radius / Textures[i].rt.descriptor.height);
				commandBuffer.SetGlobalVector(BlurShaderVariable.Offset, offset);

				Blitter.BlitTexture(commandBuffer, Textures[i], Textures[i + 1], Material, BlurShaderPass.DualKawaseDownsamle);
			}

			for (int i = Textures.Length - 1; i > 0; i--)
			{
				Vector2 offset = new(radius / Textures[i].rt.descriptor.width, radius / Textures[i].rt.descriptor.height);
				commandBuffer.SetGlobalVector(BlurShaderVariable.Offset, offset);

				Blitter.BlitTexture(commandBuffer, Textures[i], Textures[i - 1], Material, BlurShaderPass.DualKawaseUpsample);
			}
		}
	}
}
