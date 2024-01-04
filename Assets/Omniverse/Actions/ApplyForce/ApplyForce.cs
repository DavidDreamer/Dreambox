using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class ApplyForce: Action<ApplyForceDesc>
	{
		public ApplyForce(ApplyForceDesc desc): base(desc)
		{
		}

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
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
