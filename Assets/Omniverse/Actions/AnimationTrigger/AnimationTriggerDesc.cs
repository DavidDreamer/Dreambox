using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class AnimationTriggerDesc: IActionDesc
	{
		[field: SerializeField]
		public string Name { get; private set; }
	}
}
