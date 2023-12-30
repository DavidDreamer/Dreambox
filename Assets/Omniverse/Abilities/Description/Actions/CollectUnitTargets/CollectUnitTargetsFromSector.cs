using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class CollectUnitTargetsFromSector: CollectUnitTargets
	{
		[field: SerializeField]
		private float Radius { get; set; }

		[field: SerializeField]
		[field: Range(0, 360)]
		private float Angle { get; set; }

		public override IEnumerable<Unit> GetUnits(AbilityContext context)
		{
			return PhysicsHelper.GetUnitsInSector(context.Caster.Position,
				context.Caster.Direction,
				Radius,
				Angle,
				LayerMask);
		}
	}
}
