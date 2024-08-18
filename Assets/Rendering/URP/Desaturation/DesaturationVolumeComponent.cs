using UnityEngine.Rendering;

namespace Dreambox.Rendering.URP
{
	public class DesaturationVolumeComponent : VolumeComponent
	{
		public ClampedFloatParameter Factor { get; } = new(0, 0, 1);
	}
}
