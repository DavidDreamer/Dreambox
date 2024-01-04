using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class Delay: Action<DelayDesc>
	{
		public Delay(DelayDesc desc): base(desc)
		{
		}

		public override async UniTask Perform(ExecutionContext context, CancellationToken token) =>
			await UniTask.Delay(TimeSpan.FromSeconds(Desc.Duration), cancellationToken: token);
	}
}
