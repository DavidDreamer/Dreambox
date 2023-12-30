using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Omniverse.Abilities.Runtime
{
	public class SpawnVisualEffect: IAction
	{
		[field: SerializeField]
		private VisualEffect VisualEffect { get; set; }
		
		[field: SerializeField]
		private float Time { get; set; }
		
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			Vector3 position = context.Points.First();

			VisualEffect visualEffect = Object.Instantiate(VisualEffect, position, Quaternion.identity);

			Object.Destroy(visualEffect, Time);

			return UniTask.CompletedTask;
		}
	}
}
