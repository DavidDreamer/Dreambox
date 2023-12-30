using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Math;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class MoveToTarget: IAction
	{
		[field: SerializeField]
		private float Speed { get; set; }

		[field: SerializeField]
		private float LethalHeight { get; set; }

		public async UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Unit unit = context.Caster;
			
			ParabolicTrajectory3D projectileTrajectory = context.Trajectories.First();
			Vector3 targetPosition = projectileTrajectory.End;

			Vector3 direction = (targetPosition - unit.Position).normalized;
			unit.Direction = new Vector3(direction.x, 0, direction.z);

			context.Caster.Locked = true;

			Vector3 startPosition = unit.Position;

			float duration = projectileTrajectory.Parameters.Time;
			float time = 0f;

			while (time < duration)
			{
				await UniTask.NextFrame(token);

				time = Mathf.MoveTowards(time, duration, Time.deltaTime * Speed);

				unit.Position = projectileTrajectory.EvaluatePosition(time / duration);
			}

			context.Caster.Locked = false;

			float deltaHeight = startPosition.y - targetPosition.y;
			if (deltaHeight >= LethalHeight)
			{
				var damage = new ChangeResourceData
				{
					ResourceID = 0,
					Amount = -1
				};

				context.Caster.ChangeResource(damage);
			}
		}
	}
}
