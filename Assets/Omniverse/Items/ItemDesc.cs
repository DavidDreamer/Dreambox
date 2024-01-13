using UnityEngine;

namespace Omniverse
{
	public abstract class ItemDesc: ScriptableObject
	{
		[field: SerializeField]
		public ItemPresenter Prefab { get; private set; }
	}
}
