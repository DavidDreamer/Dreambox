using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class ApplyForce: IAction
	{
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				Vector3 force = (unit.Presenter.transform.position - context.Caster.Presenter.transform.position)
				                .normalized *
				                UnityEngine.Random.Range(10, 30);

				unit.AddForce(force);
			}

			return UniTask.CompletedTask;
		}
	}
}
