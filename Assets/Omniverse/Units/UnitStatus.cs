using System;

namespace Omniverse
{
	[Flags]
	public enum UnitStatus
	{
		None = 0,
		Stunned = 0x1,
		Rooted = 0x2
	}
}
