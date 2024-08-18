using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public partial class OutlineRendererFeature : CustomRendererFeature<OutlineConfig, OutlinePass>
	{
		public HashSet<OutlineRenderer> Renderers { get; } = new();

		public override OutlinePass CreatePass() => new(this);

		public void AddRenderer(OutlineRenderer outlineRenderer)
		{
			Renderers.Add(outlineRenderer);
		}

		public void RemoveRenderer(OutlineRenderer outlineRenderer)
		{
			Renderers.Remove(outlineRenderer);
		}

		public void Clear() => Renderers.Clear();

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (Renderers.Count == 0)
			{
				return;
			}

			base.AddRenderPasses(renderer, ref renderingData);
		}
	}
}
