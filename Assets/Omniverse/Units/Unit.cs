using System.Collections.Generic;
using System.Threading;
using Omniverse.Abilities;
using UnityEngine;

namespace Omniverse
{
	public class Unit
	{
		public UnitDesc Desc { get; }

		public int FactionID { get; set; }

		public Dictionary<int, Resource> Resources { get; }

		public List<Ability> Abilities { get; } = new();

		public List<Effect> Effects { get; } = new();

		public bool IsDead { get; private set; }

		public bool Locked { get; set; }

		public UnitPresenter Presenter { get; set; }

		private CancellationTokenSource DeathCancellationTokenSource { get; set; } = new();

		public CancellationToken DeathCancellationToken => DeathCancellationTokenSource.Token;

		public UnitStatus Status { get; private set; }

		public Unit(UnitDesc desc, int factionID)
		{
			Desc = desc;
			FactionID = factionID;
			
			Resources = new Dictionary<int, Resource>();
			foreach (ResourceDesc resourceDesc in Desc.Resources)
			{
				Resources.Add(resourceDesc.ID, new Resource(resourceDesc));
			}

			foreach (AbilityDesc abilityDesc in Desc.Abilities)
			{
				var ability = new Ability(abilityDesc, this);
				Abilities.Add(ability);
			}
		}

		public void FixedTick()
		{
			foreach (var resource in Resources)
			{
				resource.Value.FixedTick();
			}

			UpdateEffects(Time.fixedDeltaTime);
		}

		public void Tick()
		{
			if (IsDead)
			{
				return;
			}

			if (Locked)
			{
				return;
			}
			
			Presenter.Tick();
		}

		private void UpdateEffects(float deltaTime)
		{
			Status = UnitStatus.None;

			for (var i = 0; i < Effects.Count; ++i)
			{
				Effect effect = Effects[i];
				effect.Tick(deltaTime);

				if (effect.OutOfTime)
				{
					Effects.RemoveAt(i);
					i--;
				}
				else
				{
					Status |= effect.Desc.UnitStatus;
				}
			}
		}

		public void ChangeResource(ChangeResourceData data)
		{
			Resource resource = Resources[data.ResourceID];

			resource.Change(data.Amount);
		}

		public void AddForce(Vector3 force) => Presenter.AddForce(force);

		public void ApplyEffect(Effect effect)
		{
			Effects.Add(effect);
		}

		internal void Die()
		{
			IsDead = true;

			DeathCancellationTokenSource.Cancel();
			DeathCancellationTokenSource.Dispose();
			DeathCancellationTokenSource = null;

			Presenter.OnDeath();
		}
	}
}
