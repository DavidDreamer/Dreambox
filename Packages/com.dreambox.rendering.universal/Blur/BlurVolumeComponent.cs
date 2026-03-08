using UnityEngine.Rendering;

namespace Dreambox.Rendering.Universal
{
	public class BlurVolumeComponent : VolumeComponent
	{
		public ClampedFloatParameter Factor { get; } = new(0, 0, 1);
	}
}
