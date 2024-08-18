using System;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.URP
{
	public abstract class CustomRenderFeaturePass<TRenderFeature> : ScriptableRenderPass, IDisposable
	{
		protected TRenderFeature RenderFeature { get; private set; }

		public virtual void Initialize(TRenderFeature renderFeature)
		{
			RenderFeature = renderFeature;
		}

		public abstract void Dispose();
	}
}