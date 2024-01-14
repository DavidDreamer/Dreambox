using System;
using UnityEngine;

namespace Omniverse.Abilities
{
	[Serializable]
	public class Cast
	{
		[field: SerializeField]
		public float Time { get; private set; }
		
		[field: SerializeField]
		public string AnimationTrigger { get; private set; }
	}
}
