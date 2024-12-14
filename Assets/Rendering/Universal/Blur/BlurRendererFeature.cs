using Dreambox.Rendering.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Dreambox.Rendering.Universal
{
	public class BlurRendererFeature : PostProcessRenderFeature<BlurRenderPass>
	{
		[field: SerializeField]
		public BlurSettings Settings { get; private set; }

		public float Factor { get; private set; }

		protected override BlurRenderPass CreatePass() => new(Settings, Material);

		//var blurVolumeComponent = VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();

		//	if (blurVolumeComponent is null || blurVolumeComponent.active is false)
		//	{
		//		return;
		//	}

		//	Factor = blurVolumeComponent.Factor.value;
	}
}
