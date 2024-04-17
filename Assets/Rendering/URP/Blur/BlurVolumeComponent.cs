using UnityEngine.Rendering;

namespace Dreambox.Rendering.URP
{
	public class BlurVolumeComponent: VolumeComponent
	{
		public ClampedFloatParameter Factor { get; } = new(0, 0, 1);
	}
}
