using System;
using UnityEngine;

namespace Omniverse.Actions
{
	[Serializable]
	public class CollectUnitTargetsFromSphereDesc: CollectUnitTargetsDesc
	{
		[field: SerializeField]
		public float Radius { get; private set; }
	}
}
