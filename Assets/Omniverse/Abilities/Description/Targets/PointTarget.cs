using UnityEngine;

namespace Omniverse.Abilities.Description
{
	public class PointTarget: ITarget
	{
		[field: SerializeField]
		public float Range { get; private set; }
	}
}
