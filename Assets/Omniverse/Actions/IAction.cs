using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Actions
{
	public interface IAction
	{
		UniTask Perform(ExecutionContext context, CancellationToken token);
	}
}
