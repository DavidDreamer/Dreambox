using System;

namespace Omniverse.Abilities.Description
{
	[Flags]
	public enum UnitTargetTypeFlags
	{
		Self = 0,
		Ally = 0x1,
		Enemy = 0x2
	}
}
