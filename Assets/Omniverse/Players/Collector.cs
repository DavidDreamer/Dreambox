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
				var itemPresenter = Colliders[i].GetComponent<ItemPresenter>();

				if (itemPresenter == null)
				{
					continue;
				}

				var currencyItem = itemPresenter.Item as CurrencyItem;

				if (currencyItem is null)
				{
					continue;
				}
				
				if (currencyItem.CanBeConsumed() is false)
				{
					continue;
				}

				ItemManager.Consume(currencyItem, Player.Unit);
			}
		}
	}
}
