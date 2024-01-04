using System.Threading;
using Cysharp.Threading.Tasks;

namespace Omniverse.Actions
{
	public abstract class Action<T>: IAction where T: IActionDesc
	{
		protected T Desc { get; }

		protected Action(T desc)
		{
			Desc = desc;
		}

		public abstract UniTask Perform(ExecutionContext context, CancellationToken token);
	}
}
