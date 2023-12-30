using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class CollectUnitTargetsFromSphere: CollectUnitTargets
	{
		[field: SerializeField]
		private float Radius { get; set; }

		public override IEnumerable<Unit> GetUnits(AbilityContext context)
		{
			Vector3 position = context.Points.First();
			return PhysicsHelper.GetUnitsInSphere(position, Radius, LayerMask);
		}
	}
}
