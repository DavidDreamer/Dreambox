using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	[HideInInspector]
	[SupportedOnRenderPipeline(typeof(RenderPipelineAsset))]
	public class OutlineShaders : IRenderPipelineResources
	{
		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/Outline/Mask", SearchType.ShaderName)]
		public Shader Mask { get; private set; }

		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/Outline/JumpFlood", SearchType.ShaderName)]
		public Shader JumpFlood { get; private set; }

		[field: SerializeField]
		[field: ResourcePath("Outline/JumpFlood.compute")]
		public ComputeShader JumpFloodCompute { get; private set; }

		public int version => 0;

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild => true;
	}
}
