using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class Suicide: IAction
	{
		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			context.Caster.Suicide();
			return UniTask.CompletedTask;
		}
	}
}
