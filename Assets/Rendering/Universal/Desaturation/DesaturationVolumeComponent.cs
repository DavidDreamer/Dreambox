using UnityEngine.Rendering;

namespace Dreambox.Rendering.Universal
{
	public class DesaturationVolumeComponent : VolumeComponent
	{
		public ClampedFloatParameter Factor { get; } = new(0, 0, 1);
	}
}
