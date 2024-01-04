using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class MoveToTargetDesc: IActionDesc
	{
		[field: SerializeField]
		public float Speed { get; private set; }

		[field: SerializeField]
		public float LethalHeight { get; private set; }
	}
}
