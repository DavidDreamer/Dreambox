using UnityEngine.Rendering.Universal;
using UnityEngine;
using System;

namespace Dreambox.Rendering.URP
{
	public abstract class RendererFeature<TConfig, TPass> : ScriptableRendererFeature
		where TConfig : RendererFeatureConfig
		where TPass : ScriptableRenderPass, IDisposable
	{
		[field: SerializeField]
		public TConfig Config { get; private set; }

		public TPass Pass { get; private set; }

		public abstract TPass CreatePass();

		public override void Create()
		{
			Pass = CreatePass();
			Pass.renderPassEvent = Config.RenderPassEvent;
		}

		protected override void Dispose(bool disposing)
		{
			Pass.Dispose();
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

