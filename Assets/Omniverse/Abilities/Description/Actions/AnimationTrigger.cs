using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Core;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class AnimationTrigger: IAction
	{
		[field: SerializeField]
		public string Name { get; private set; }

		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				unit.Presenter.Animator.SetTrigger(AnimatorParameter.Get(Name));
			}
			
			return UniTask.CompletedTask;
		}
	}
}
