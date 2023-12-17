using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class UnitSpawnData
	{
		[field: SerializeField]
		public UnitDescriptor Descriptor { get; private set; }
		
		[field: SerializeField]
		[field: Faction]
		public int Faction { get; private set; }
	}
}
