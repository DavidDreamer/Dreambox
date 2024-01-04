using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Core;
using JetBrains.Annotations;

namespace Omniverse.Actions
{
	[UsedImplicitly]
	public class AnimationTrigger: Action<AnimationTriggerDesc>
	{
		public AnimationTrigger(AnimationTriggerDesc desc): base(desc)
		{
		}

		public override UniTask Perform(ExecutionContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				unit.Presenter.Animator.SetTrigger(AnimatorParameter.Get(Desc.Name));
			}

			return UniTask.CompletedTask;
		}
	}
}
