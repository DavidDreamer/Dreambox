using System.Collections.Generic;
using System.Linq;
using Dreambox.Core.Pooling;
using UnityEngine;
using VContainer.Unity;

namespace Omniverse
{
	[UnityEngine.Scripting.Preserve]
	public class UnitManager: IFixedTickable, IPostFixedTickable, ITickable
	{
		private PrefabPool PrefabPool { get; }

		private ItemManager ItemManager { get; }

		private List<Unit> Units { get; } = new();

		public UnitManager(PrefabPool prefabPool, ItemManager itemManager)
		{
			PrefabPool = prefabPool;
			ItemManager = itemManager;
		}

		public Unit Spawn(UnitSpawnData data) => Spawn(data.Description, data.FactionID);

		public Unit Spawn(UnitDescription description, int factionID)
		{
			var unit = new Unit(description, factionID)
			{
				Presenter = PrefabPool.Take(description.Presentation.Prefab)
			};

			unit.Presenter.Bind(unit);

			Units.Add(unit);

			return unit;
		}

		public void Despawn(Unit unit)
		{
			PrefabPool.Return(unit.Description.Presentation.Prefab, unit.Presenter);
			Units.Remove(unit);
		}

		public void FixedTick()
		{
			for (var i = 0; i < Units.Count; i++)
			{
				Units[i].FixedTick();
			}
		}

		public void PostFixedTick()
		{
			for (var i = 0; i < Units.Count; i++)
			{
				Unit unit = Units[i];

				if (ShouldBeKilled(unit))
				{
					Kill(unit);
				}
			}

			bool ShouldBeKilled(Unit unit)
			{
				if (unit.Alive)
				{
					return false;
				}
				
				return unit.Resources.Values.Any(resource => resource.Vital && resource.OutOf);
			}
		}

		private void Kill(Unit unit)
		{
			unit.OnDied();
			DropLoot(unit);
		}

		private void DropLoot(Unit unit)
		{
			foreach (LootDescription loot in unit.Description.Loot)
			{
				float random = Random.Range(0f, 1f);

				if (loot.DropChance <= random)
				{
					continue;
				}

				ItemManager.Spawn(loot.Item, unit.Presenter.transform.position, Quaternion.identity, null);
			}
		}

		public void Tick()
		{
			for (var i = 0; i < Units.Count; i++)
			{
				Units[i].Tick();
			}
		}

		public void Clear()
		{
			Units.Clear();
		}
	}
}
