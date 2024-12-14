using Dreambox.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public abstract class CustomRendererFeature<TPass> : ScriptableRendererFeature where TPass : ScriptableRenderPass
	{
		[field: SerializeField]
		public RenderPassEvent RenderPassEvent { get; private set; }

		[field: SerializeField]
		[field: EnumFlags]
		public CameraType CameraType { get; private set; } = CameraType.SceneView | CameraType.Game;

		public TPass Pass { get; private set; }

		public abstract TPass CreatePass();

		public override void Create()
		{
			Pass = CreatePass();
			Pass.renderPassEvent = RenderPassEvent;
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (CameraType.HasFlag(renderingData.cameraData.cameraType))
			{
				renderer.EnqueuePass(Pass);
			}
		}
	}
}
