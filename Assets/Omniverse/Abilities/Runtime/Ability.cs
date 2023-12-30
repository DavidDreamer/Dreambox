﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Core;
using Omniverse.Abilities.Description;
using Descriptor = Omniverse.Abilities.Description.Ability;

namespace Omniverse.Abilities.Runtime
{
	public class Ability
	{
		public Descriptor Descriptor { get; }

		public Unit Unit { get; }
		
		public Cooldown Cooldown { get; }
		
		public List<Unit> Targets { get; } = new();

		public AbilityContext Context { get; }

		public bool AwaitsTarget { get; set; }

		public bool InProcess { get; private set; }
		
		public Ability(Descriptor descriptor, Unit unit)
		{
			Descriptor = descriptor;
			Unit = unit;

			Context = new AbilityContext(unit);
			Cooldown = Descriptor.Cooldown == null ? null : new Cooldown(Descriptor.Cooldown);
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

			foreach (Cost abilityCostDescription in Descriptor.Cost)
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

			if (!string.IsNullOrEmpty(Descriptor.Cast.AnimationTrigger))
			{
				Unit.Presenter.Animator.SetTrigger(AnimatorParameter.Get(Descriptor.Cast.AnimationTrigger));
			}

			await UniTask.Delay(TimeSpan.FromSeconds(Descriptor.Cast.Time), cancellationToken: token);
			
			foreach (Cost cost in Descriptor.Cost)
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
			
			foreach (IAction action in Descriptor.Actions)
			{
				await action.Perform(Context, token);
			}

			Targets.Clear();
			Context.Clear();
		}
	}
}