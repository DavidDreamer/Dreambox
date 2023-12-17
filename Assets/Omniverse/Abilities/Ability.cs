using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Core;

namespace Omniverse
{
	public class Ability
	{
		public AbilityDescription Description { get; }

		public Unit Unit { get; }
		
		public Cooldown Cooldown { get; }
		
		public List<Unit> Targets { get; } = new();

		public AbilityContext Context { get; }

		public bool AwaitsTarget { get; set; }

		public bool InProcess { get; private set; }
		
		public Ability(AbilityDescription description, Unit unit)
		{
			Description = description;
			Unit = unit;

			Context = new AbilityContext(unit);
			Cooldown = Description.Cooldown == null ? null : new Cooldown(Description.Cooldown);
		}

		public AbilityCastError CanBeCasted()
		{
			if (Cooldown is not null && Cooldown.IsActive)
			{
				return AbilityCastError.IsOnCooldown;
			}

			if (InProcess)
			{
				return AbilityCastError.AlreadyInProcess;
			}

			foreach (AbilityCostDescription abilityCostDescription in Description.Cost)
			{
				if (!Unit.Resources.ContainsKey(abilityCostDescription.ResourceID))
				{
					return AbilityCastError.NotEnoughResources;
				}

				if (Unit.Resources[abilityCostDescription.ResourceID].Amount.Value < abilityCostDescription.Amount)
				{
					return AbilityCastError.NotEnoughResources;
				}
			}
			
			return AbilityCastError.None;
		}
		
		public async UniTask Cast(CancellationToken token)
		{
			InProcess = true;

			if (!string.IsNullOrEmpty(Description.Cast.AnimationTrigger))
			{
				Unit.Presenter.Animator.SetTrigger(AnimatorParameter.Get(Description.Cast.AnimationTrigger));
			}

			await UniTask.Delay(TimeSpan.FromSeconds(Description.Cast.Time), cancellationToken: token);
			
			foreach (AbilityCostDescription cost in Description.Cost)
			{
				var data = new ChangeResourceData
				{
					ResourceID = cost.ResourceID,
					Amount = -cost.Amount
				};

				Unit.ChangeResource(data);
			}
			
			Cooldown?.ActivateAsync(token);

			InProcess = false;
			
			foreach (IAction action in Description.Actions)
			{
				await action.Perform(Context, token);
			}

			Targets.Clear();
			Context.Clear();
		}
	}
}
