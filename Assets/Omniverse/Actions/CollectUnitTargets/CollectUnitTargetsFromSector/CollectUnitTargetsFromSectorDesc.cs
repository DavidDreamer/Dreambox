using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class CollectUnitTargetsFromSectorDesc: CollectUnitTargetsDesc
	{
		[field: SerializeField]
		public float Radius { get; private set; }

		[field: SerializeField]
		[field: Range(0, 360)]
		public float Angle { get; private set; }
	}
}
