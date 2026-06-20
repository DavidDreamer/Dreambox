using UnityEngine;
using UnityEngine.Rendering;
using Dreambox.Rendering.Core;

namespace Dreambox.Rendering.HDRP
{
	internal class OutlinePipelineGraphics : OutlinePipeline
	{
		public OutlinePipelineGraphics(Shader shader, OutlineVariant[] variants) : base(shader, variants)
		{
		}

		public override void Initialize(CommandBuffer commandBuffer)
		{
			RTHandle startBuffer = Iterations % 2 == 0 ? JumpFlood2RT : JumpFlood1RT;
			Blitter.BlitTexture(commandBuffer, MaskRT, startBuffer, Material, OutlineShaderPass.Initialize);
		}

		public override void JumpFlood(CommandBuffer commandBuffer, RTHandle source, RTHandle target, int stepWidth)
		{
			commandBuffer.SetGlobalFloat(OutlineShaderVariable.StepWidth, stepWidth);
			Blitter.BlitTexture(commandBuffer, source, target, Material, OutlineShaderPass.JumpFlood);
		}
	}
}
