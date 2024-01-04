using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class KillUnits: Action<KillUnitsDesc>
	{
		public KillUnits(KillUnitsDesc desc): base(desc)
		{
		}
		
		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				unit.Die();
			}
	
			return UniTask.CompletedTask;
		}
	}
}
