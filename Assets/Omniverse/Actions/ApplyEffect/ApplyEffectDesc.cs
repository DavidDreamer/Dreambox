using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class ApplyEffectDesc: IActionDesc
	{
		[field: SerializeField]
		public EffectDescription Effect { get; private set; }
	}
}
