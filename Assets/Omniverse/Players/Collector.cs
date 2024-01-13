using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Omniverse
{
	public class Collector: IFixedTickable
	{
		[Inject]
		private Player Player { get; set; }
		
		[Inject]
		private ItemManager ItemManager { get; set; }
		
		private CollectorSettings Settings { get; }

		private Collider[] Colliders { get; }

		public Collector(CollectorSettings settings)
		{
			Settings = settings;
			Colliders = new Collider[Settings.Capacity];
		}
		
		public void FixedTick()
		{
			Transform playerUnitTransform = Player.Unit.Presenter.Hitbox.transform;

			int count = Physics.OverlapSphereNonAlloc(playerUnitTransform.position, Settings.Radius, Colliders,
				Settings.LayerMask,
				QueryTriggerInteraction.Collide);

			for (int i = 0; i < count; ++i)
			{
				var consumableItem = Colliders[i].GetComponent<IConsumableItem>();

				if (consumableItem == null)
				{
					continue;
				}

				if (consumableItem.CanBeConsumed() is false)
				{
					continue;
				}

				ItemManager.Consume(consumableItem, Player.Unit);
			}
		}
	}
}
