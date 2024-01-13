using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class LootDescription
	{
		[field: SerializeField]
		public ItemDesc Item { get; private set; }
		
		[field: SerializeField]
		[field: Range(0f, 1f)]
		public float DropChance { get; private set; }
	}
}
