using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class MoveInDirectionDesc: IActionDesc
	{
		[field: SerializeField]
		public Vector3 Direction { get; private set; }

		[field: SerializeField]
		public float Distance { get; private set; }

		[field: SerializeField]
		public float Duration { get; private set; }
	}
}
