using UnityEngine;

namespace Omniverse
{
	public class UnitTarget: ITarget
	{
		[field: SerializeField]
		public float Range { get; private set; }
	}
}
