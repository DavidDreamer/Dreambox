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
			//TODO: insta kill
			var data = new ChangeResourceData
			{
				ResourceID = 0,
				Amount = 1000
			};
				
			foreach (Unit unit in context.Units)
			{
				unit.ChangeResource(data);
			}
	
			return UniTask.CompletedTask;
		}
	}
}
