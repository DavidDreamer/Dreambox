using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Abilities.Runtime
{
	[Serializable]
	public class ClearUnits: IAction
	{
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			context.Units.Clear();
			return UniTask.CompletedTask;
		}
	}
}
