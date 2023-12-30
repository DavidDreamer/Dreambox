using System;

namespace Omniverse.Abilities.Description
{
	[Flags]
	public enum UnitTargetTypeFlags
	{
		Self = 0x1,
		Ally = 0x2,
		Enemy = 0x4
	}
}
