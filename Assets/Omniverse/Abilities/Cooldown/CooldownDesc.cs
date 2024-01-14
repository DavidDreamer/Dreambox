using System;
using UnityEngine;

namespace Omniverse.Abilities
{
	[Serializable]
	public class CooldownDesc
	{
		[field: SerializeField]
		public float Time { get; private set; }
	}
}
