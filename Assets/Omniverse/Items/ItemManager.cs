using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Omniverse
{
	[UnityEngine.Scripting.Preserve]
	public class ItemManager
	{
		//private List<Item> Items { get; } = new();
		
		[Inject]
		private IObjectResolver ObjectResolver { get; set; }

		public void Spawn(ItemDesc desc, Vector3 position, Quaternion rotation, Transform parent)
		{
			IItem item = desc.Build();
			ObjectResolver.Inject(item);

			ItemPresenter presenter = Object.Instantiate(desc.Prefab, position, rotation, parent);
			ObjectResolver.InjectGameObject(presenter.gameObject);
			
			presenter.Item = item;
			item.Presenter = presenter;
		}

		public void Consume(IConsumableItem item, Unit unit)
		{
			item.OnConsumed(unit);
		}
	}
}
