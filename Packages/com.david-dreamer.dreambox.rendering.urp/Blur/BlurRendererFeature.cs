using UnityEngine;

namespace Dreambox.Rendering.Universal
{
	public class BlurRendererFeature : PostProcessRenderFeature<BlurRenderPass>
	{
		[field: SerializeField]
		public int SampleCount { get; set; } = 32;

		[field: SerializeField]
		public float Radius { get; set; } = 10;

		[field: SerializeField]
		[field: Range(1, 4)]
		public int Downsample { get; set; } = 2;

		public float Factor { get; private set; }

		protected override BlurRenderPass CreatePass() => new(this, Material);

		//var blurVolumeComponent = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();

		//	if (blurVolumeComponent is null || blurVolumeComponent.active is false)
		//	{
		//		return;
		//	}

		//	Factor = blurVolumeComponent.Factor.value;
	}
}
