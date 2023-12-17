using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse
{
	public interface IAction
	{
		UniTask Perform(AbilityContext context, CancellationToken token);
	}
}
