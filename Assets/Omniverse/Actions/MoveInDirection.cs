using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Omniverse
{
	[Serializable]
	public class MoveInDirection: IAction
	{
		[field: SerializeField]
		private Vector3 Direction { get; set; }

		[field: SerializeField]
		private float Distance { get; set; }
		
		[field: SerializeField]
		private float Duration { get; set; }

		public async UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Unit unit = context.Caster;

			Vector3 worldSpaceDirection = unit.Rotation * Direction;
			Vector3 sourcePoint = unit.Position;
			Vector3 targetPoint = sourcePoint + worldSpaceDirection * Distance;
			if (NavMesh.SamplePosition(targetPoint, out NavMeshHit h, float.MaxValue, 1))
			{
				targetPoint = h.position;
			}

			float time = 0f;
			
			while (time < Duration)
			{
				await UniTask.WaitForFixedUpdate(token);

				time = Mathf.Min(time + Time.fixedDeltaTime, Duration);
				
				Vector3 currentPosition = Vector3.Lerp(sourcePoint, targetPoint, time / Duration);
				
				if (NavMesh.SamplePosition(currentPosition, out NavMeshHit hit, float.MaxValue, 1))
				{
					currentPosition = hit.position;
				}

				unit.Position = currentPosition;
			}
		}
	}
}
