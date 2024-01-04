using System.Collections.Generic;
using System.Threading;
using Omniverse.Abilities.Runtime;
using UnityEngine;

namespace Omniverse
{
	public class Unit
	{
		public Vector3 Position
		{
			get => Presenter.transform.position;

			set => Presenter.transform.position = value;
		}

		public Quaternion Rotation
		{
			get => Presenter.transform.rotation;

			set => Presenter.transform.rotation = value;
		}

		public Vector3 Direction
		{
			get => Presenter.transform.forward;

			set => Presenter.transform.forward = value;
		}
		
		public UnitDescriptor Descriptor { get; }

		public int FactionID { get; set; }

		public Dictionary<int, Resource> Resources { get; }

		public List<Ability> Abilities { get; } = new();

		public List<Effect> Effects { get; } = new();

		public bool Alive { get; set; } = true;

		public bool Locked { get; set; }

		public UnitPresenter Presenter { get; set; }

		private CancellationTokenSource DeathCancellationTokenSource { get; set; } = new();

		public CancellationToken DeathCancellationToken => DeathCancellationTokenSource.Token;

		public UnitStatus Status { get; private set; }

		public Unit(UnitDescriptor descriptor, int factionID)
		{
			Descriptor = descriptor;
			FactionID = factionID;
			
			Resources = new Dictionary<int, Resource>();
			foreach (ResourceDescriptor resourceDescription in Descriptor.Resources)
			{
				Resources.Add(resourceDescription.ID, new Resource(resourceDescription));
			}

			foreach (Abilities.Description.Ability abilityDescription in Descriptor.AbilityDescriptions)
			{
				var ability = new Ability(abilityDescription, this);
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
			if (!Alive)
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
					Status |= effect.Descriptor.UnitStatus;
				}
			}
		}

		public void ChangeResource(ChangeResourceData data)
		{
			Resource resource = Resources[data.ResourceID];

			resource.Change(data.Amount);

			if (resource.Vital && resource.OutOf)
			{
				Die();
			}
		}

		public void AddForce(Vector3 force) => Presenter.AddForce(force);

		public void ApplyEffect(Effect effect)
		{
			Effects.Add(effect);
		}

		public void Die()
		{
			Alive = false;

			DeathCancellationTokenSource.Cancel();
			DeathCancellationTokenSource.Dispose();
			DeathCancellationTokenSource = null;

			Presenter.OnDeath();
		}
	}
}
