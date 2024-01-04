using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class ChangeResource: Action<ChangeResourceDesc>
	{
		public ChangeResource(ChangeResourceDesc desc): base(desc)
		{
		}

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				var data = new ChangeResourceData
				{
					ResourceID = Desc.ResourceID,
					Source = context.Caster,
					Amount = Desc.Amount
				};

				unit.ChangeResource(data);
			}

			return UniTask.CompletedTask;
		}
	}
}
