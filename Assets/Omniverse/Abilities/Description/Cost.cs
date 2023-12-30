using System;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class Cost
	{
		[field: SerializeField]
		[field: Resource]
		public int ResourceID { get; private set; }
		
		[field: SerializeField]
		public float Amount { get; set; }
		
		[field: SerializeField]
		public CostMode Mode { get; private set; }
	}
}
