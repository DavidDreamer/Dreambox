using UnityEngine;
using VContainer;

namespace Omniverse
{
	[UnityEngine.Scripting.Preserve]
	public class ItemManager
	{
		//private List<Item> Items { get; } = new();
		
		[Inject]
		private FactionManager FactionManager { get; set; }
		
		public void Spawn(ItemDesc description, Vector3 position, Quaternion rotation, Transform parent)
		{
			ItemPresenter presenter = Object.Instantiate(description.Prefab, position, rotation, parent);
			//var item = new Item(description, presenter);
		}

		public void Consume(IConsumableItem item, Unit unit)
		{
			item.OnConsumed(unit);
		}
	}
}
