using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public abstract class CollectUnitTargets<T>: Action<T> where T : CollectUnitTargetsDesc
	{
		protected CollectUnitTargets(T desc): base(desc)
		{
		}

		public abstract IEnumerable<Unit> GetUnits(ExecutionContext context);

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			bool selfCastAllowed = Desc.UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Self);
			bool allyCastAllowed = Desc.UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Ally);
			bool enemyCastAllowed = Desc.UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Enemy);

			foreach (Unit unit in GetUnits(context))
			{
				if (unit == context.Caster && !selfCastAllowed)
				{
					continue;
				}

				bool hasSameFaction = unit.FactionID == context.Caster.FactionID;

				switch (hasSameFaction)
				{
					case true when !allyCastAllowed:
					case false when !enemyCastAllowed:
						continue;
				}

				context.Units.Add(unit);
			}

			return UniTask.CompletedTask;
		}
	}
}
