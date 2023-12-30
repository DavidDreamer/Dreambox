using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class Delay: IAction
	{
		[field: SerializeField]
		public float Duration { get; private set; }

		public async UniTask Perform(AbilityContext context, CancellationToken token) =>
			await UniTask.Delay(TimeSpan.FromSeconds(Duration), cancellationToken: token);
	}
}
