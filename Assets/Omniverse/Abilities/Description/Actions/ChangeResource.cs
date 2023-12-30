using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class ChangeResource: IAction
	{
		[field: SerializeField]
		[field: Resource]
		public int ResourceID { get; private set; }

		[field: SerializeField]
		public int Amount { get; private set; }

		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				var data = new ChangeResourceData
				{
					ResourceID = ResourceID,
					Source = context.Caster,
					Amount = Amount
				};

				unit.ChangeResource(data);
			}

			return UniTask.CompletedTask;
		}
	}
}
