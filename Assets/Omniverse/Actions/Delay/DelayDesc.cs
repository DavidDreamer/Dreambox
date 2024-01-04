using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class DelayDesc: IActionDesc
	{
		[field: SerializeField]
		public float Duration { get; private set; }
	}
}
