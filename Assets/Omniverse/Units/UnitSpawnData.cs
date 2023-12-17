using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public struct UnitSpawnData
	{
		[field: SerializeField]
		public UnitDescriptor Descriptor { get; private set; }
		
		[field: SerializeField]
		[field: Faction]
		public int FactionID { get; private set; }
	}
}
