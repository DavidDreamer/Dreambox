using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Dreambox.Rendering.Core
{
	[Serializable]
	[HideInInspector]
	[SupportedOnRenderPipeline(typeof(RenderPipelineAsset))]
	public class BlurShaders : IRenderPipelineResources
	{
		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/PostProcessing/Blur/Box", SearchType.ShaderName)]
		public Shader Box { get; private set; }

		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/PostProcessing/Blur/Gaussian", SearchType.ShaderName)]
		public Shader Gaussian { get; private set; }

		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/PostProcessing/Blur/Kawase", SearchType.ShaderName)]
		public Shader Kawase { get; private set; }

		[field: SerializeField]
		[field: ResourcePath("Hidden/Dreambox/PostProcessing/Blur/DualKawase", SearchType.ShaderName)]
		public Shader DualKawase { get; private set; }

		public int version => 0;

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild => true;
	}
}
