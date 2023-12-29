using UnityEngine;

namespace Omniverse
{
	public static class FactionUtils
	{
		public static LayerMask GetHostileLayerMask(this Faction faction) => 1 & ~(1 << faction.Layer);
	}
}
