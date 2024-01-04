using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class ClearUnits: Action<ClearUnitsDesc>
	{
		public ClearUnits(ClearUnitsDesc desc): base(desc)
		{
		}
		
		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			context.Units.Clear();
			return UniTask.CompletedTask;
		}
	}
}
