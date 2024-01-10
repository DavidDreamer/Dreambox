using UnityEngine;

namespace Omniverse
{
	[CreateAssetMenu(menuName = nameof(Omniverse) + "/" + nameof(ItemDescription), fileName = nameof(ItemDescription))]
	public class ItemDescription: ScriptableObject
	{
		[field: SerializeField]
		public ItemPresenter Prefab { get; private set; }
	}
}
