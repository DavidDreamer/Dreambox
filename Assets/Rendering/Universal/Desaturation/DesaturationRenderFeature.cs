using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class DesaturationRenderFeature : PostProcessRenderFeature<DesaturationRenderPass>
	{
		public float Factor { get; private set; }

		protected override DesaturationRenderPass CreatePass() => new(Material);
	}
}
