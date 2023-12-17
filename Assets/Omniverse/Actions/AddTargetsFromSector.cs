using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse
{
	[Serializable]
	public class AddTargetsFromSector: IAction
	{
		[field: SerializeField]
		private float Radius { get; set; }

		[field: SerializeField]
		[field: Range(0, 360)]
		private float Angle { get; set; }

		[field: SerializeField]
		private LayerMask LayerMask { get; set; }

		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Unit caster = context.Caster;

			var units = PhysicsHelper.GetUnitsInSector(caster.Position,
				caster.Direction,
				Radius,
				Angle,
				LayerMask);
			
			foreach (Unit unit in units)
			{
				if (unit == context.Caster)
				{
					continue;
				}

				context.Units.Add(unit);
			}

			return UniTask.CompletedTask;
		}
	}
}
