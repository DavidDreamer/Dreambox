using System;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class CooldownDescriptor
	{
		[field: SerializeField]
		public float Time { get; private set; }
	}
}
