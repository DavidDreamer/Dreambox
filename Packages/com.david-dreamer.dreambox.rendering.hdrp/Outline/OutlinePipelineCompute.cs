using UnityEngine;
using UnityEngine.Rendering;
using Dreambox.Rendering.Core;

namespace Dreambox.Rendering.HDRP
{
	internal class OutlinePipelineCompute : OutlinePipeline
	{
		private const float NUM_THREADS = 16;

		private ComputeShader ComputeShader { get; }

		private int ThreadGroupsX { get; set; }
		private int ThreadGroupsY { get; set; }
		private int ThreadGroupsZ { get; set; } = 1;

		public OutlinePipelineCompute(Shader shader, ComputeShader computeShader, OutlineVariant[] variants) : base(shader, variants)
		{
			ComputeShader = computeShader;
		}

		public override void Initialize(CommandBuffer commandBuffer)
		{
			int width = JumpFlood1RT.rt.width;
			int height = JumpFlood1RT.rt.height;

			Vector2 resolution = new(width, height);
			commandBuffer.SetComputeVectorParam(ComputeShader, "Resolution", resolution);

			ThreadGroupsX = Mathf.CeilToInt(width / NUM_THREADS);
			ThreadGroupsY = Mathf.CeilToInt(height / NUM_THREADS);

			commandBuffer.SetComputeTextureParam(ComputeShader, OutlineKernel.Initialize, "MaskTexture", MaskRT);

			RTHandle startBuffer = Iterations % 2 == 0 ? JumpFlood2RT : JumpFlood1RT;
			commandBuffer.SetComputeTextureParam(ComputeShader, OutlineKernel.Initialize, "JumpFloodTargetTexture", startBuffer);

			commandBuffer.DispatchCompute(ComputeShader, OutlineKernel.Initialize, ThreadGroupsX, ThreadGroupsY, ThreadGroupsZ);
		}

		public override void JumpFlood(CommandBuffer commandBuffer, RTHandle source, RTHandle target, int stepWidth)
		{
			commandBuffer.SetComputeTextureParam(ComputeShader, OutlineKernel.JumpFlood, "JumpFloodSourceTexture", source);
			commandBuffer.SetComputeTextureParam(ComputeShader, OutlineKernel.JumpFlood, "JumpFloodTargetTexture", target);
			commandBuffer.SetComputeIntParam(ComputeShader, OutlineShaderVariable.StepWidth, stepWidth);

			commandBuffer.DispatchCompute(ComputeShader, OutlineKernel.JumpFlood, ThreadGroupsX, ThreadGroupsY, ThreadGroupsZ);
		}
	}
}
