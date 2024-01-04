using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Math;
using UnityEngine;

namespace Omniverse.Actions
{
	public class ExecutionContext
	{
		private IAction[] Actions { get; }

		public Unit Caster { get; }

		public List<Unit> Units { get; } = new();

		public List<Vector3> Points { get; } = new();

		public List<ParabolicTrajectory3D> Trajectories { get; } = new();

		public ExecutionContext(Unit caster, IActionDesc[] actionDescs)
		{
			Caster = caster;
			Actions = actionDescs.Select(desc => desc.Build()).ToArray();
		}

		public async UniTask PerformAsync(CancellationToken token)
		{
			foreach (IAction action in Actions)
			{
				await action.Perform(this, token);
			}

			Clear();
		}

		private void Clear()
		{
			Units.Clear();
			Points.Clear();
			Trajectories.Clear();
		}
	}
}
