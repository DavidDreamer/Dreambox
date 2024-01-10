using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public struct UnitSpawnData
	{
		[field: SerializeField]
		public UnitDescription Description { get; private set; }
		
		[field: SerializeField]
		[field: FactionID]
		public int FactionID { get; private set; }
	}
}
