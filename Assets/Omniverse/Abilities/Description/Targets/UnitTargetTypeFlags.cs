using System;

namespace Omniverse
{
	[Flags]
	public enum UnitTargetTypeFlags
	{
		Self = 0,
		Ally = 0x1,
		Enemy = 0x2
	}
}
