using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Abilities.Runtime
{
	public interface IAction
	{
		UniTask Perform(AbilityContext context, CancellationToken token);
	}
}
