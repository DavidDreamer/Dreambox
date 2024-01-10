using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Omniverse
{
	[Preserve]
	public class ItemManager
	{
		private List<Item> Items { get; } = new();

		public void Spawn(ItemDescription description, Vector3 position, Quaternion rotation, Transform parent)
		{
			ItemPresenter presenter = Object.Instantiate(description.Prefab, position, rotation, parent);
			var item = new Item(description, presenter);
		}
	}
}
