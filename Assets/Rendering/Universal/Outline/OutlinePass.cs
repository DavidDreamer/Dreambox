// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class OutlinePass : ScriptableRenderPass, IDisposable
	{
		private static class ShaderVariables
		{
			public static int StepWidth { get; } = Shader.PropertyToID(nameof(StepWidth));

			public static int VariantsBuffer { get; } = Shader.PropertyToID(nameof(VariantsBuffer));

			public static int VariantIndex { get; } = Shader.PropertyToID(nameof(VariantIndex));

			public static int BaseMap { get; } = Shader.PropertyToID($"_{nameof(BaseMap)}");
		}

		private static class ShaderPasses
		{
			public const int Mask = 0;

			public const int Init = 1;

			public const int JumpFlood = 2;

			public const int Decode = 3;
		}

		private OutlineRenderer RendererFeature { get; }

		private OutlineRendererConfig Config { get; }

		private Material Material { get; set; }

		private RTHandle Mask;

		private RTHandle JumpBuffer1;

		private RTHandle JumpBuffer2;

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
			Mask?.Release();
			JumpBuffer1?.Release();
			JumpBuffer2?.Release();
			VariantsBuffer?.Release();
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			base.Configure(cmd, cameraTextureDescriptor);

			var renderTextureDescriptor =
				new RenderTextureDescriptor(cameraTextureDescriptor.width, cameraTextureDescriptor.height)
				{
					graphicsFormat = GraphicsFormat.R8_UInt
				};

			RenderingUtils.ReAllocateIfNeeded(ref Mask, renderTextureDescriptor);

			renderTextureDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
			RenderingUtils.ReAllocateIfNeeded(ref JumpBuffer1, renderTextureDescriptor);
			RenderingUtils.ReAllocateIfNeeded(ref JumpBuffer2, renderTextureDescriptor);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			using var scope = new CommandBufferContextScope(context, "Outline");
			CommandBuffer commandBuffer = scope.CommandBuffer;

			RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

			UpdateConfigs();
			PerformMasking();
			PerformJumpFloodPasses();
			Decode();

			void PerformMasking()
			{
				CoreUtils.SetRenderTarget(commandBuffer, Mask, ClearFlag.Color);

				foreach (OutlineTarget outlineRenderer in RendererFeature.Targets)
				{
					commandBuffer.SetGlobalInteger(ShaderVariables.VariantIndex, outlineRenderer.Variant + 1);
					Texture baseMap = outlineRenderer.Renderer.sharedMaterial.GetTexture(ShaderVariables.BaseMap);
					commandBuffer.SetGlobalTexture(ShaderVariables.BaseMap, baseMap);
					commandBuffer.DrawRenderer(outlineRenderer.Renderer, Material, 0, ShaderPasses.Mask);
				}
			}

			void PerformJumpFloodPasses()
			{
				float maxPixelWidth = JumpBuffer1.rt.height * MaxOffsetWidthOfAllConfigs;
				int iterations = Mathf.CeilToInt(Mathf.Log(maxPixelWidth, 2f)) - 1;

				RTHandle startBuffer = iterations % 2 == 0 ? JumpBuffer2 : JumpBuffer1;
				commandBuffer.Blit(Mask, startBuffer, Material, ShaderPasses.Init);

				for (int i = iterations; i >= 0; i--)
				{
					float stepWidth = Mathf.Pow(2, i);
					commandBuffer.SetGlobalFloat(ShaderVariables.StepWidth, stepWidth);

					if (i % 2 == 1)
					{
						commandBuffer.Blit(JumpBuffer1, JumpBuffer2, Material, ShaderPasses.JumpFlood);
					}
					else
					{
						commandBuffer.Blit(JumpBuffer2, JumpBuffer1, Material, ShaderPasses.JumpFlood);
					}
				}
			}

			void Decode() => commandBuffer.Blit(JumpBuffer1, cameraColorTargetHandle, Material, ShaderPasses.Decode);
		}

		private void UpdateConfigs()
		{
			MaxOffsetWidthOfAllConfigs = Config.Variants.Max(config => config.Width);
			VariantsBuffer.SetData(Config.Variants);
		}
	}
}
