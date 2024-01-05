using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class CollectUnitTargetsFromSphere: CollectUnitTargets<CollectUnitTargetsFromSphereDesc>
	{
		public CollectUnitTargetsFromSphere(CollectUnitTargetsFromSphereDesc desc): base(desc)
		{
		}

		public override IEnumerable<Unit> GetUnits(ExecutionContext context)
		{
			Vector3 position = context.Points.First();
			return PhysicsHelper.GetUnitsInSphere(position, Desc.Radius, GlobalSettings.Instance.HitboxLayerMask);
		}
	}
}
