using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class Faction
	{
		[field: SerializeField]
		public string Name { get; private set; }
	}
}
