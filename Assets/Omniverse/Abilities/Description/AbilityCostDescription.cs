using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class AbilityCostDescription
	{
		[field: SerializeField]
		[field: Resource]
		public int ResourceID { get; private set; }
		
		[field: SerializeField]
		public float Amount { get; set; }
		
		[field: SerializeField]
		public AbilityCostMode Mode { get; private set; }
	}
}
