using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class LaunchProjectileDesc: IActionDesc
	{
		[field: SerializeField]
		public Projectile Projectile { get; private set; }

		[field: SerializeField]
		public float Force { get; private set; }
	}
}
