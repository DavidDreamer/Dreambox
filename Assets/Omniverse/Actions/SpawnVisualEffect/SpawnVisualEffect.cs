using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class SpawnVisualEffect: Action<SpawnVisualEffectDesc>
	{
		public SpawnVisualEffect(SpawnVisualEffectDesc desc): base(desc)
		{
		}

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			Vector3 position = context.Points.First();

			VisualEffect visualEffect = Object.Instantiate(Desc.VisualEffect, position, Quaternion.identity);

			Object.Destroy(visualEffect, Desc.Time);

			return UniTask.CompletedTask;
		}
	}
}
