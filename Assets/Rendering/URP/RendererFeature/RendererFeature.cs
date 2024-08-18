using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace Dreambox.Rendering.URP
{
	public abstract class RendererFeature<TRenderFeature, TConfig, TPass> : ScriptableRendererFeature
		where TRenderFeature : RendererFeature<TRenderFeature, TConfig, TPass>
		where TConfig : RendererFeatureConfig
		where TPass : CustomRenderFeaturePass<TRenderFeature>, new()
	{
		[field: SerializeField]
		public TConfig Config { get; private set; }

		public TPass Pass { get; private set; }

		public override void Create()
		{
			Pass?.Dispose();
			Pass = new TPass();
			Pass.Initialize((TRenderFeature)this);
			Pass.renderPassEvent = Config.RenderPassEvent;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Pass?.Dispose();
			Pass = null;
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (Config.CameraType.HasFlag(renderingData.cameraData.cameraType))
			{
				renderer.EnqueuePass(Pass);
			}
		}
	}
}
