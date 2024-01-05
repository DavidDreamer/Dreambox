using System.Collections.Generic;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class CollectUnitTargetsFromSector: CollectUnitTargets<CollectUnitTargetsFromSectorDesc>
	{
		public CollectUnitTargetsFromSector(CollectUnitTargetsFromSectorDesc desc): base(desc)
		{
		}

		public override IEnumerable<Unit> GetUnits(ExecutionContext context)
		{
			return PhysicsHelper.GetUnitsInSector(context.Caster.Position,
				context.Caster.Direction,
				Desc.Radius,
				Desc.Angle,
				GlobalSettings.Instance.HitboxLayer);
		}
	}
}
