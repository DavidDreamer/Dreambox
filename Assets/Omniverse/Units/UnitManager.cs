using System.Collections.Generic;
using Dreambox.Core.Pooling;
using UnityEngine;
using VContainer.Unity;

namespace Omniverse
{
	[UnityEngine.Scripting.Preserve]
	public class UnitManager: IFixedTickable, ITickable
	{
		private PrefabPool PrefabPool { get; }

		private ItemManager ItemManager { get; }
		
		private List<Unit> Units { get; } = new();

		public Unit Spawn(UnitSpawnData data) => Spawn(data.Descriptor, data.FactionID);

		public UnitManager(PrefabPool prefabPool, ItemManager itemManager)
		{
			PrefabPool = prefabPool;
			ItemManager = itemManager;
		}
		
		public Unit Spawn(UnitDescriptor descriptor, int factionID)
		{
			var unit = new Unit(descriptor, factionID)
			{
				Presenter = PrefabPool.Take(descriptor.Presentation.Prefab)
			};
			
			unit.Presenter.Bind(unit);

			Units.Add(unit);

			return unit;
		}
		
		public void Despawn(Unit unit)
		{
			DropLoot(unit);
			
			PrefabPool.Return(unit.Descriptor.Presentation.Prefab, unit.Presenter);
			Units.Remove(unit);
		}
		
		private void DropLoot(Unit unit)
		{
			UnitDescriptor descriptor = unit.Descriptor;
			
			foreach (LootDescription loot in descriptor.Loot)
			{
				float random = Random.Range(0f, 1f);

				if (loot.DropChance <= random)
				{
					continue;
				}

				ItemManager.Spawn(loot.Item, unit.Position, Quaternion.identity, null);
			}
		}
		
		public void FixedTick()
		{
			for (var i = 0; i < Units.Count; i++)
			{
				Units[i].FixedTick();
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
