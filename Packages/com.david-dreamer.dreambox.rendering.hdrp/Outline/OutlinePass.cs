using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Dreambox.Rendering.Core;

namespace Dreambox.Rendering.HDRP
{
	public class OutlinePass : CustomPass
	{
		[field: SerializeField]
		private OutlinePipelineType PipelineType { get; set; }

		[field: SerializeField]
		private Shader Shader { get; set; }

		[field: SerializeField]
		private ComputeShader ComputeShader { get; set; }

		[field: SerializeField]
		public OutlineVariant[] Variants { get; private set; }

		public HashSet<OutlineRenderer> Renderers { get; } = new();

		private OutlinePipeline Pipeline { get; set; }

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			Pipeline = PipelineType is OutlinePipelineType.Compute && SystemInfo.supportsComputeShaders
				? new OutlinePipelineCompute(Shader, ComputeShader, Variants)
				: new OutlinePipelineGraphics(Shader, Variants);
		}

		protected override void Cleanup()
		{
			Pipeline.Dispose();
		}

		protected override void Execute(CustomPassContext context)
		{
			CommandBuffer commandBuffer = context.cmd;

			Pipeline.RefreshVariants(Variants);

			Pipeline.Mask(commandBuffer, Renderers);
			Pipeline.Initialize(commandBuffer);
			Pipeline.JumpFlood(commandBuffer);
			Pipeline.Decode(commandBuffer, context.cameraColorBuffer);
		}
	}
}
