// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class OutlineRenderPass : ScriptableRenderPass
	{
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

		private class MaskingPassData
		{
			public Material Material;

			public HashSet<OutlineTarget> Targets;

			public TextureHandle Target;
		}

		private class InitPassData
		{
			public Material Material;

			public TextureHandle Source;
		}

		private class JumpFloodPassData
		{
			public Material Material;

			public TextureHandle Source;

			public int Iteration;
		}

		private class DecodingPassData
		{
			public Material Material;

			public TextureHandle Source;
		}

		private Material Material { get; }

		HashSet<OutlineTarget> Targets { get; }

		private float Width { get; }

		public OutlineRenderPass(Material material, HashSet<OutlineTarget> targets, float width)
		{
			Material = material;
			Targets = targets;
			Width = width;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			float maxPixelWidth = Screen.height * Width;
			int iterations = Mathf.CeilToInt(Mathf.Log(maxPixelWidth, 2f)) - 1;

			var resourceData = frameData.Get<UniversalResourceData>();
			var outlineData = frameData.Create<OutlineData>();

			TextureHandle cameraColorTexture = resourceData.cameraColor;
			TextureDesc cameraColorTextureDesc = cameraColorTexture.GetDescriptor(renderGraph);

			using (IUnsafeRenderGraphBuilder builder = renderGraph.AddUnsafePass<MaskingPassData>("Outline.Masking", out var data))
			{
				data.Material = Material;
				data.Targets = Targets;

				RenderTextureDescriptor textureDesc =
					new(cameraColorTextureDesc.width, cameraColorTextureDesc.height, RenderTextureFormat.RInt, 0, 0, RenderTextureReadWrite.Default);
				TextureHandle targetTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.Mask", true);

				data.Target = outlineData.Mask = targetTexture;
				builder.UseTexture(targetTexture);

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((MaskingPassData data, UnsafeGraphContext context) => ExecuteMasking(data, context));
			}

			using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<InitPassData>("Outline.Init", out var data))
			{
				data.Material = Material;

				builder.UseTexture(outlineData.Mask);
				data.Source = outlineData.Mask;

				RenderTextureDescriptor textureDesc =
					new(cameraColorTextureDesc.width, cameraColorTextureDesc.height, RenderTextureFormat.ARGBFloat, 0, 0, RenderTextureReadWrite.Default);
				outlineData.JumpBuffer1 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.JumpBuffer1", true);
				outlineData.JumpBuffer2 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDesc, "Outline.JumpBuffer2", true);

				TextureHandle startBuffer = iterations % 2 == 0 ? outlineData.JumpBuffer2 : outlineData.JumpBuffer1;
				builder.SetRenderAttachment(startBuffer, 0);

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((InitPassData data, RasterGraphContext context) => ExecuteInit(data, context));
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

					data.Material = Material;

					builder.UseTexture(source);
					data.Source = source;
					data.Iteration = 1;

					builder.SetRenderAttachment(target, 0);

					builder.AllowGlobalStateModification(true);

					builder.SetRenderFunc((JumpFloodPassData data, RasterGraphContext context) => ExecuteJumpFlood(data, context));
				}
			}

			using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass<DecodingPassData>("Outline.Decoding", out var data))
			{
				data.Material = Material;

				data.Source = outlineData.JumpBuffer1;
				builder.UseTexture(outlineData.JumpBuffer1);

				builder.SetRenderAttachment(cameraColorTexture, 0);

				builder.AllowGlobalStateModification(true);

				builder.SetRenderFunc((DecodingPassData data, RasterGraphContext context) => ExecuteDecoding(data, context));
			}
		}

		private static void ExecuteMasking(MaskingPassData data, UnsafeGraphContext context)
		{
			CommandBuffer commandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

			commandBuffer.SetRenderTarget(data.Target);
			commandBuffer.ClearRenderTarget(true, true, Color.clear);

			foreach (OutlineTarget target in data.Targets)
			{
				commandBuffer.SetGlobalInteger(OutlineShaderVariables.Variant, target.Variant + 1);
				Texture baseMap = target.Material.HasTexture(OutlineShaderVariables.BaseMap) ?
					target.Material.GetTexture(OutlineShaderVariables.BaseMap) : Texture2D.whiteTexture;
				commandBuffer.SetGlobalTexture(OutlineShaderVariables.BaseMap, baseMap);
				commandBuffer.DrawMesh(target.Mesh, target.Matrix, data.Material, 0, ShaderPasses.Mask);
			}
		}

		private static void ExecuteInit(InitPassData data, RasterGraphContext context)
		{
			RasterCommandBuffer commandBuffer = context.cmd;
			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), data.Material, ShaderPasses.Init);
		}

		private static void ExecuteJumpFlood(JumpFloodPassData data, RasterGraphContext context)
		{
			RasterCommandBuffer commandBuffer = context.cmd;

			float stepWidth = Mathf.Pow(2, data.Iteration);
			commandBuffer.SetGlobalFloat(OutlineShaderVariables.StepWidth, stepWidth);

			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), data.Material, ShaderPasses.JumpFlood);
		}

		private static void ExecuteDecoding(DecodingPassData data, RasterGraphContext context)
		{
			RasterCommandBuffer commandBuffer = context.cmd;
			Blitter.BlitTexture(commandBuffer, data.Source, new Vector4(1, 1, 0, 0), data.Material, ShaderPasses.Decode);
		}
	}
}
