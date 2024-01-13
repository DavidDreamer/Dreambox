using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class ResourceItemDesc: ItemDesc
	{
		[field: SerializeField]
		[field: Resource]
		private int ResourceID { get; set; }
		
		[field: SerializeField]
		private int Amount { get; set; }
	}
}
