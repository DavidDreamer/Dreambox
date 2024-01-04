using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class AddTargetSelf: Action<AddTargetSelfDesc>
	{
		public AddTargetSelf(AddTargetSelfDesc desc): base(desc)
		{
		}

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			context.Units.Add(context.Caster);
			return UniTask.CompletedTask;
		}
	}
}
