using System;
using Dreambox.Core;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class Faction
	{
		[field: SerializeField]
		public string Name { get; private set; }
		
		[field: SerializeField]
		[field: Layer]
		public int Layer { get; private set; }
	}
}
