// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class OutlinePass : ScriptableRenderPass, IDisposable
	{
		private static class ShaderVariables
		{
			public static int StepWidth { get; } = Shader.PropertyToID(nameof(StepWidth));

			public static int VariantsBuffer { get; } = Shader.PropertyToID(nameof(VariantsBuffer));

			public static int Variant { get; } = Shader.PropertyToID(nameof(Variant));

			public static int BaseMap { get; } = Shader.PropertyToID($"_{nameof(BaseMap)}");
		}

		private static class ShaderPasses
		{
			public const int Mask = 0;

			public const int Init = 1;

			public const int JumpFlood = 2;

			public const int Decode = 3;
		}

		private class OutlineData : ContextItem
		{
			public TextureHandle Mask;

			public TextureHandle JumpBuffer1;

			public TextureHandle JumpBuffer2;

			public override void Reset()
			{
				Mask = TextureHandle.nullHandle;
				JumpBuffer1 = TextureHandle.nullHandle;
				JumpBuffer2 = TextureHandle.nullHandle;
			}
		}

		private class JumpFloodPassData
		{
			public TextureHandle Source;

			public int Iteration;
		}

		private class PassData
		{
			public TextureHandle Source;
		}

		private OutlineRenderer RendererFeature { get; }

		private OutlineRendererConfig Config { get; }

		private Material Material { get; set; }

		private ComputeBuffer VariantsBuffer { get; set; }

		private float MaxOffsetWidthOfAllConfigs { get; set; }

		public OutlinePass(OutlineRenderer rendererFeature)
		{
			RendererFeature = rendererFeature;

			Config = RendererFeature.Config;

			Material = CoreUtils.CreateEngineMaterial(Config.Shader);

			VariantsBuffer = new ComputeBuffer(Config.Variants.Length, Marshal.SizeOf<OutlineVariant>());

			Material.SetBuffer(ShaderVariables.VariantsBuffer, VariantsBuffer);
		}

		public void Dispose()
		{
			CoreUtils.Destroy(Material);
			VariantsBuffer?.Release();
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UpdateConfigs();

			float maxPixelWidth = Screen.height * MaxOffsetWidthOfAllConfigs;
			int iterations = Mathf.CeilToInt(Mathf.Log(maxPixelWidth, 2f)) - 1;

			var resourceData = frameData.Get<UniversalResourceData>();
			var outlineData = frameData.Create<OutlineData>();

			TextureHandle cameraColorTexture = resourceData.cameraColor;
			TextureDesc cameraColorTextureDesc = cameraColorTexture.GetDescriptor(renderGraph);

			using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Outline.Masking", out var data))
			{
				RenderTextureDescriptor textureDesc =
					new(cameraColorTextureDesc.width, cameraColorTextureDesc.height, RenderTextureFormat.RInt, 0, 0, RenderTextureReadWrite.Default);
				TextureHandle targetTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.Mask", true);
				builder.SetRenderAttachment(targetTexture, 0);

				outlineData.Mask = targetTexture;

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteMasking(context));
			}

			using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Outline.Init", out var data))
			{
				builder.UseTexture(outlineData.Mask);
				data.Source = outlineData.Mask;

				RenderTextureDescriptor textureDesc =
					new(cameraColorTextureDesc.width, cameraColorTextureDesc.height, RenderTextureFormat.ARGBFloat, 0, 0, RenderTextureReadWrite.Default);
				outlineData.JumpBuffer1 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.JumpBuffer1", true);
				outlineData.JumpBuffer2 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.JumpBuffer2", true);

				TextureHandle startBuffer = iterations % 2 == 0 ? outlineData.JumpBuffer2 : outlineData.JumpBuffer1;
				builder.SetRenderAttachment(startBuffer, 0);

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteInit(context, data));
			}

			for (int i = iterations; i >= 0; i--)
			{
				using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<JumpFloodPassData>("Outline.JumpFlood", out var data))
				{
					TextureHandle source, target;
					if (i % 2 == 1)
					{
						source = outlineData.JumpBuffer1;
						target = outlineData.JumpBuffer2;
					}
					else
					{
						source = outlineData.JumpBuffer2;
						target = outlineData.JumpBuffer1;
					}

					builder.UseTexture(source);
					data.Source = source;
					data.Iteration = 1;

					builder.SetRenderAttachment(target, 0);

					builder.AllowGlobalStateModification(true);

					builder.SetRenderFunc((JumpFloodPassData data, RasterGraphContext context) => ExecuteJumpFlood(context, data));
				}
			}

			using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<PassData>("Outline.Decoding", out var data))
			{
				data.Source = outlineData.JumpBuffer1;
				builder.UseTexture(outlineData.JumpBuffer1);

				builder.SetRenderAttachment(cameraColorTexture, 0);

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteDecoding(context, data));
			}
		}

		private void ExecuteMasking(RasterGraphContext context)
		{
			RasterCommandBuffer commandBuffer = context.cmd;

			commandBuffer.ClearRenderTarget(true, true, Color.clear);

			foreach (OutlineTarget outlineRenderer in RendererFeature.Targets)
			{
				commandBuffer.SetGlobalInteger(ShaderVariables.Variant, outlineRenderer.Variant + 1);
				Texture baseMap = outlineRenderer.Renderer.sharedMaterial.GetTexture(ShaderVariables.BaseMap);
				//commandBuffer.SetGlobalTexture(ShaderVariables.BaseMap, baseMap);
				commandBuffer.DrawRenderer(outlineRenderer.Renderer, Material, 0, ShaderPasses.Mask);
			}
		}

		private void ExecuteInit(RasterGraphContext context, PassData data)
		{
			RasterCommandBuffer commandBuffer = context.cmd;
			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), Material, ShaderPasses.Init);
		}

		private void ExecuteJumpFlood(RasterGraphContext context, JumpFloodPassData data)
		{
			RasterCommandBuffer commandBuffer = context.cmd;

			float stepWidth = Mathf.Pow(2, data.Iteration);
			commandBuffer.SetGlobalFloat(ShaderVariables.StepWidth, stepWidth);

			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), Material, ShaderPasses.JumpFlood);
		}

		private void ExecuteDecoding(RasterGraphContext context, PassData data)
		{
			RasterCommandBuffer commandBuffer = context.cmd;
			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), Material, ShaderPasses.Decode);
		}

		private void UpdateConfigs()
		{
			MaxOffsetWidthOfAllConfigs = Config.Variants.Max(config => config.Width);
			VariantsBuffer.SetData(Config.Variants);
		}
	}
}
