using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Runtime
{
	[Serializable]
	public class AddTargetsFromSphere: IAction
	{
		[field: SerializeField]
		private float Radius { get; set; }

		[field: SerializeField]
		private LayerMask LayerMask { get; set; }

		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Vector3 position = context.Points.First();
			var units = PhysicsHelper.GetUnitsInSphere(position, Radius, LayerMask);
			context.Units.AddRange(units);

			return UniTask.CompletedTask;
		}
	}
}
