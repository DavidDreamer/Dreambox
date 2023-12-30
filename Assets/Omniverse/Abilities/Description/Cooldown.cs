using System;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class Cooldown
	{
		[field: SerializeField]
		public float Time { get; private set; }
	}
}
