using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse
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
