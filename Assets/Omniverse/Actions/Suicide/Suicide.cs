using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class Suicide: Action<SuicideDesc>
	{
		public Suicide(SuicideDesc desc): base(desc)
		{
		}
		
		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			context.Caster.Suicide();
			return UniTask.CompletedTask;
		}
	}
}
