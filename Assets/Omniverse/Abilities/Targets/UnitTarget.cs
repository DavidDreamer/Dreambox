using UnityEngine;

namespace Omniverse.Abilities
{
	public class UnitTarget: ITarget
	{
		[field: SerializeField]
		public UnitTargetTypeFlags Type { get; private set; }
		
		[field: SerializeField]
		public float Range { get; private set; }
	}
}
