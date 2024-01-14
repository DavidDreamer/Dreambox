using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class ApplyEffectDesc: IActionDesc
	{
		[field: SerializeField]
		public EffectDesc Effect { get; private set; }
	}
}
