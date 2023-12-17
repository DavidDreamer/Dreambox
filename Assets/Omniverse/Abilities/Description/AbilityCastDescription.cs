using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class AbilityCastDescription
	{
		[field: SerializeField]
		public float Time { get; private set; }
		
		[field: SerializeField]
		public string AnimationTrigger { get; private set; }
	}
}
