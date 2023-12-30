using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Abilities.Runtime
{
	[Serializable]
	public class AddTargetSelf: IAction
	{
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			context.Units.Add(context.Caster);
			return UniTask.CompletedTask;
		}
	}
}
