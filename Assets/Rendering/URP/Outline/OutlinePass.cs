// Copyright (c) Saber BGS 2022. All rights reserved.
// ---------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	[Serializable]
	public class OutlinePass: ScriptableRenderPass
	{
		private static class ShaderVariables
		{
			public static int StepWidth { get; } = Shader.PropertyToID(nameof(StepWidth));
			public static int UnscaledTime { get; } = Shader.PropertyToID(nameof(UnscaledTime));
			public static int ConfigIndex { get; } = Shader.PropertyToID(nameof(ConfigIndex));
			public static int VariantsBuffer { get; } = Shader.PropertyToID(nameof(VariantsBuffer));
		}

		private static class ShaderPasses
		{
			public const int Mask = 0;
			public const int Init = 1;
			public const int JumpFlood = 2;
			public const int Decode = 3;
		}

		private Material Material { get; set; }

		private RTHandle Mask;

		private RTHandle JumpBuffer1;

		private RTHandle JumpBuffer2;

		private ComputeBuffer VariantsBuffer { get; set; }
		private float MaxOffsetWidthOfAllConfigs { get; set; }

		private HashSet<Renderer> Renderers { get; } = new();
		
		private OutlineConfig Config { get; set; }

		public OutlinePass(OutlineConfig config)
		{
			Config = config;
			
			Material = CoreUtils.CreateEngineMaterial(config.Shader);

			VariantsBuffer = new ComputeBuffer(Config.Variants.Length, Marshal.SizeOf<OutlineVariant>());

			Material.SetBuffer(ShaderVariables.VariantsBuffer, VariantsBuffer);
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			base.Configure(cmd, cameraTextureDescriptor);

			var renderTextureDescriptor =
				new RenderTextureDescriptor(cameraTextureDescriptor.width, cameraTextureDescriptor.height);
			
			renderTextureDescriptor.graphicsFormat = GraphicsFormat.R8_UInt;
			
			RenderingUtils.ReAllocateIfNeeded(ref Mask, renderTextureDescriptor);
			
			renderTextureDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
			RenderingUtils.ReAllocateIfNeeded(ref JumpBuffer1, renderTextureDescriptor);
			RenderingUtils.ReAllocateIfNeeded(ref JumpBuffer2, renderTextureDescriptor);
		}
		
		public void Dispose()
		{
			CoreUtils.Destroy(Material);
			Mask.Release();
			JumpBuffer1.Release();
			JumpBuffer2.Release();
			VariantsBuffer.Release();
		}

		public void AddRenderer(Renderer renderer)
		{
			Renderers.Add(renderer);
		}

		public void RemoveRenderer(Renderer renderer)
		{
			Renderers.Remove(renderer);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (Renderers.Count == 0)
			{
				return;
			}

			using var scope = new CommandBufferContextScope(context, "Outline");
			CommandBuffer commandBuffer = scope.CommandBuffer;

			RTHandle cameraColorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
			
			UpdateData();
			PerformMasking();
			PerformJumpFloodPasses();
			Decode();
			
			void UpdateData()
			{
				commandBuffer.SetGlobalFloat(ShaderVariables.UnscaledTime, Time.unscaledTime);
				UpdateConfigs();
			}
			
			void PerformMasking()
			{
				CoreUtils.SetRenderTarget(commandBuffer, Mask, ClearFlag.Color);

				foreach (Renderer renderer in Renderers)
				{
					commandBuffer.SetGlobalInteger(ShaderVariables.ConfigIndex, (int)1);
					commandBuffer.DrawRenderer(renderer, Material, 0, ShaderPasses.Mask);
				}
			}

			void PerformJumpFloodPasses()
			{
				// Calculate the number of jump flood passes needed for the current outline width
				// + 1.0f to handle half pixel inset of the init pass and antialiasing
				float maxPixelWidth = JumpBuffer1.rt.height * MaxOffsetWidthOfAllConfigs;
				int jumps = Mathf.CeilToInt(Mathf.Log(maxPixelWidth + 1f, 2f));
				int iterations = jumps - 1;
				
				var startBuffer = iterations % 2 == 0 ? JumpBuffer2 : JumpBuffer1;
				commandBuffer.Blit(Mask, startBuffer, Material, ShaderPasses.Init);
				
				for (int i = iterations; i >= 0; i--)
				{
					// Calculate appropriate jump width for each iteration
					// + 0.5 is just me being cautious to avoid any floating point math rounding errors
					float stepWidth = Mathf.Pow(2, i) + 0.5f;
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

		public void Clear() => Renderers?.Clear();
	}
}
