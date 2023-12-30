using UnityEngine;

namespace Omniverse
{
	public class TrajectoryTarget: ITarget
	{
		[field: SerializeField]
		public float Range { get; private set; }

		[field: SerializeField]
		public float Height { get; private set; }
	}
}
