using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Omniverse.Abilities.Description
{
	[Serializable]
	public class ApplyEffect: IAction
	{
		[field: SerializeField]
		private EffectDescriptor Effect { get; set; }

		public UniTask Perform(AbilityContext context, CancellationToken token)
		{
			foreach (Unit unit in context.Units)
			{
				var effect = new Effect(Effect);
				
				unit.ApplyEffect(effect);
			}
			
			return UniTask.CompletedTask;
		}
	}
}
