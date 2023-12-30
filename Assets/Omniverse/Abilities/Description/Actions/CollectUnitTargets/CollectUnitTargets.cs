using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public abstract class CollectUnitTargets: IAction
	{
		[field: SerializeField]
		protected LayerMask LayerMask { get; private set; }
		
		[field: SerializeField]
		private UnitTargetTypeFlags UnitTargetTypeFlags { get; set; }
		
		public abstract IEnumerable<Unit> GetUnits(AbilityContext context);
		
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			bool selfCastAllowed = UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Self);
			bool allyCastAllowed = UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Ally);
			bool enemyCastAllowed = UnitTargetTypeFlags.HasFlag(UnitTargetTypeFlags.Enemy);
			
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
