using UnityEngine;

namespace Omniverse
{
	public class PointTarget: ITarget
	{
		[field: SerializeField]
		public float Range { get; private set; }
	}
}
