using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Dreambox.Core;
using ExecutionContext = Omniverse.Actions.ExecutionContext;

namespace Omniverse.Abilities
{
	public class Ability
	{
		public AbilityDesc Desc { get; }

		public Unit Unit { get; }
		
		public Cooldown Cooldown { get; }
		
		public ExecutionContext Context { get; }

		public bool AwaitsTarget { get; set; }

		public bool InProcess { get; private set; }
		
		public Ability(AbilityDesc desc, Unit unit)
		{
			Desc = desc;
			Unit = unit;

			Context = new ExecutionContext(unit, Desc.Actions);
			Cooldown = new Cooldown(Desc.Cooldown);
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

			foreach (CostDesc costDesc in Desc.Cost)
			{
				if (!Unit.Resources.ContainsKey(costDesc.ResourceID))
				{
					return AbilityCastError.NotEnoughResources;
				}

				if (Unit.Resources[costDesc.ResourceID].Amount.Value < costDesc.Amount)
				{
					return AbilityCastError.NotEnoughResources;
				}
			}
			
			return AbilityCastError.None;
		}
		
		public async UniTask Cast(CancellationToken token)
		{
			InProcess = true;

			if (!string.IsNullOrEmpty(Desc.Cast.AnimationTrigger))
			{
				Unit.Presenter.Animator.SetTrigger(AnimatorParameter.Get(Desc.Cast.AnimationTrigger));
			}

			await UniTask.Delay(TimeSpan.FromSeconds(Desc.Cast.Time), cancellationToken: token);
			
			foreach (CostDesc cost in Desc.Cost)
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

			await Context.PerformAsync(token);
		}
	}
}
