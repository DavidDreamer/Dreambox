using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Omniverse.Actions
{
	[Serializable]
	public class SpawnVisualEffectDesc: IActionDesc
	{
		[field: SerializeField]
		public VisualEffect VisualEffect { get; private set; }

		[field: SerializeField]
		public float Time { get; private set; }
	}
}
