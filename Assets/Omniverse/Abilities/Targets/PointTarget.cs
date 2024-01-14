using UnityEngine;

namespace Omniverse.Abilities
{
	public class PointTarget: ITarget
	{
		[field: SerializeField]
		public float Range { get; private set; }
	}
}
